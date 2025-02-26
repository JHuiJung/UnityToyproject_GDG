import os
import discord
from discord.ext import tasks
from dotenv import load_dotenv
import gspread
import datetime
from daily_topics import daily_topics

load_dotenv()

# google spreadsheet
json_file_path = os.environ.get('JSON_FILE_PATH')
gc = gspread.service_account(json_file_path)
sheet_url = os.environ.get('SPREADSHEET_URL')
sheet = gc.open_by_url(sheet_url)

worksheet = sheet.sheet1

utc = datetime.timezone.utc
daily_topic_send_time = datetime.time(hour=0, minute=0, tzinfo=utc)
daily_ranking_send_time = datetime.time(hour=12, minute=0, tzinfo=utc)

# discord bot
intents = discord.Intents.default()
intents.message_content = True

client = discord.Client(intents=intents)

@client.event
async def on_ready():   # 클라이언트가 디스코드로부터 데이터를 받을 준비 완료 시
    print(f'Logged in as {client.user}')
    send_daily_topic.start()
    send_daily_ranking.start()

@tasks.loop(time=daily_topic_send_time)
async def send_daily_topic():
    today = datetime.date.today()
    daily_topic = daily_topics[today.day - 21]
    channel = client.get_channel(1286291325616128103)
    await channel.send(f'## {daily_topic['topic']}\n**{daily_topic['question']}**')

@tasks.loop(time=daily_ranking_send_time)
async def send_daily_ranking():
    names = worksheet.col_values(2)
    max_heights = worksheet.col_values(5)
    data = list(zip(names, max_heights))
    data = data[1:]
    data.sort(key=lambda x: x[1], reverse=True)
    top_5_rankings = data[:5]

    channel = client.get_channel(1286291325616128103)
    message = '## 🏆 오늘의 랭킹\n'
    for i in range(5):
        name, ranking = top_5_rankings[i]
        message += f'- **{i + 1}등 {name}**   {ranking}m\n'
    await channel.send(message)

@client.event
async def on_message(message):  # 메시지 전송 완료 시
    if message.channel.name in ['유쾌한-전대광장', 'i-love-cake-케익-쌓기-챌린지'] or message.channel.parent.name in ['유쾌한-전대광장', 'i-love-cake-케익-쌓기-챌린지']:
        if message.author == client.user:   # 메시지를 전송한 사용자가 봇인 경우
            return

        if message.content:
            await message.add_reaction('✅')
            cell = worksheet.find(f'{message.author.id}')
            if not cell:
                worksheet.append_row([f'{message.author.id}', f'{message.author.nick[:3]}', '5', '0', '0', 'None', 'None', 'None', 'None', 'None'])
            else:
                score = worksheet.cell(cell.row, 3).value
                worksheet.update_cell(cell.row, 3, f'{int(score) + 5}')

client.run(os.environ.get('DISCORD_BOT_TOKEN'))