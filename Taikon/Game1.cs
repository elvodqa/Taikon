using System;
using System.Collections.Generic;
using System.IO;
using FontStashSharp;
using ManagedBass;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Taikon.Graphics;

namespace Taikon;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private FontSystem _fontSystem;

    private string _mapSong;
    private string _mapArtist;
    private string _mapCreator;
    private string _mapAudioFile;
    private List<string> _mapObjects;
    private string _mapFile = "./Songs/TestSong1/map1.tai";
    private int _currentMapBpm;
    private double _currentMapOffset;
    private double _currentMapLength;
    private double _currentMapTime = 0;
    private bool _isPlaying = true;
    
    private Texture2D _hitAreaCirleTexture;
    
    private Texture2D _hitObjectDonTexture;
    private Texture2D _hitObjectKaTexture;
    private int _hitObjectRadius = 165;
    
    private Texture2D _hitObjectDonBigTexture;
    private Texture2D _hitObjectKaBigTexture;
    private int _hitObjectBigRadius = 180;

    private int _songStreamHandle;
    

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        if (!Bass.Init())
        {
            throw new Exception("Bass failed to initialize!");
        }
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _fontSystem = new();
        _fontSystem.AddFont(File.ReadAllBytes("./Fonts/arial.ttf"));

        _hitAreaCirleTexture = Primitives.CreateCircleWithOutline(GraphicsDevice, _hitObjectRadius+5, 15);
        
        _mapObjects = ReadMapFile(_mapFile);
        PlaySong("Songs/TestSong1/ferrari-halloween.mp3");
    }

    protected override void Update(GameTime gameTime)
    {
        if (_isPlaying)
        {
            _currentMapTime += gameTime.ElapsedGameTime.TotalMilliseconds;
        }
        
        Input.GetKeyboardState();
        Input.GetMouseState();
        
        
        
        Input.FixScrollLater();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();
        SpriteFontBase font18 = _fontSystem.GetFont(18);
        _spriteBatch.DrawString(font18, "Song: " + _mapSong, new Vector2(10, 10), Color.White);
        _spriteBatch.DrawString(font18, "Artist: " + _mapArtist, new Vector2(10, 30), Color.White);
        _spriteBatch.DrawString(font18, "Creator: " + _mapCreator, new Vector2(10, 50), Color.White);
        _spriteBatch.DrawString(font18, "BPM: " + _currentMapBpm, new Vector2(10, 70), Color.White);
        _spriteBatch.DrawString(font18, "Audio: " + _mapAudioFile, new Vector2(10, 90), Color.White);
        _spriteBatch.DrawString(font18, "MapTime: " + _currentMapTime, new Vector2(10, 110), Color.White);
        _spriteBatch.DrawString(font18, "SongTime: " + SongPosition, new Vector2(10, 130), Color.White);
        
        _spriteBatch.Draw(
            _hitAreaCirleTexture, 
            new Vector2(_hitObjectRadius+5, Window.ClientBounds.Height/2), 
            null, Color.White, 0f, 
            new Vector2((_hitObjectRadius+5)/2, (_hitObjectRadius+5)/2), 
            1f, SpriteEffects.None, 0f);
        
        _spriteBatch.End();
        

        base.Draw(gameTime);
    }
    
    private void PlaySong(string audioFile)
    {
        _songStreamHandle = Bass.CreateStream(audioFile);
        Bass.ChannelPlay(_songStreamHandle);
    }
    
    private void StopSong()
    {
        Bass.Stop();
    }
    
    private void PauseSong()
    {
        Bass.Pause();
    }
    
    private void ResumeSong()
    {
        Bass.Start();
    }
    
    private long SongPosition
    {
        get
        {
            return Bass.ChannelGetPosition(_songStreamHandle);
        }
        set => Bass.ChannelSetPosition(_songStreamHandle, value);
        
    }

    private List<string> ReadMapFile(string mapFile)
    {
        List<string> objects = new();
        string[] lines = File.ReadAllLines(mapFile);
        bool objectsStarted = false;
        foreach (var line in lines)
        {
            if (line == "")
                continue;
            if (line.StartsWith("//"))
                continue;
            if (line.StartsWith("["))
            {
                if (line == "[Objects]")
                    objectsStarted = true;
                continue;
            }
            if (!objectsStarted)
            {
                var split = line.Split(":");
                switch (split[0])
                {
                    case "Song":
                        _mapSong = split[1].Trim();
                        break;
                    case "Artist":
                        _mapArtist = split[1].Trim();
                        break;
                    case "Creator":
                        _mapCreator = split[1].Trim();
                        break;
                    case "Audio":
                        _mapAudioFile = split[1].Trim();
                        break;
                    case "BPM":
                        _currentMapBpm = int.Parse(split[1].Trim());
                        break;
                }
            }
            else
            {
                objects.Add(line);
            }
        }

        return objects;
    }
}