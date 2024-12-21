using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using System;

public class DiscordController : MonoBehaviour
{
    private static Discord.Discord discord;

    private const string applicationID = "1319574653697658890";

    private void Awake()
    {
        discord = new Discord.Discord(long.Parse(applicationID), (ulong)CreateFlags.Default);
    }
}
