import os
import discord
from dotenv import load_dotenv
import gspread

load_dotenv()

# google spreadsheet
json_file_path = os.environ.get('JSON_FILE_PATH')
gc = gspread.service_account(json_file_path)
sheet_url = os.environ.get('SPREADSHEET_URL')
sheet = gc.open_by_url(sheet_url)

worksheet = sheet.sheet1

# discord bot
intents = discord.Intents.default()
intents.message_content = True

client = discord.Client(intents=intents)

@client.event
async def on_ready():   # 클라이언트가 디스코드로부터 데이터를 받을 준비 완료 시
    print(f'Logged in as {client.user}')

@client.event
async def on_message(message):  # 메시지 전송 완료 시
    if message.author == client.user:   # 메시지를 전송한 사용자가 봇이 아닌 경우
        return

    if message.content:
        await message.add_reaction('✅')
        cell = worksheet.find(f'{message.author.id}')
        if not cell:
            worksheet.append_row([f'{message.author.id}', f'{message.author.nick[:3]}', '5', '0', '0', '0'])
        else:
            score = worksheet.cell(cell.row, 3).value
            worksheet.update_cell(cell.row, 3, f'{int(score) + 5}')

client.run(os.environ.get('DISCORD_BOT_TOKEN'))