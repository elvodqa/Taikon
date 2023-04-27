using System;
using System.Collections.Generic;
using System.IO;
using FontStashSharp;
using ManagedBass;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Taikon.Audio;
using Taikon.Core;
using Taikon.Graphics;

namespace Taikon;

public enum GameState
{
    MainMenu,
    SongSelect,
    GamePlay,
    Editor,
}

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private FontSystem _fontSystem;

    private SongPlayer _songPlayer;

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

    private Texture2D _mainMenuEditorTexture;
    private Texture2D _mainMenuPlayTexture;
    private Texture2D _mainMenuSettingsTexture;
    private int _onHoverMovePlay = 0;
    private int _onHoverMoveEditor = 0;
    private int _onHoverMoveSettings = 0;
    private SoundEffect _buttonHoverSound;
    private SoundEffect _buttonClickSound;
    private bool _isHoveringPlay = false;

    private GameState _gameState = GameState.MainMenu;
    private SpriteFontBase font18;
    
    private Texture2D _donSplashTexture;
    private float _dontRotation = 0;

    private List<Map> Maps = new();
    private Map SelectedMap;
    private SubMap SelectedSubMap;

    private bool _focusedSettings = false;
    private bool _isVolumeChancing = false;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnResize;
        
    }

    protected override void Initialize()
    {
        _songPlayer = new();
        
        foreach (var folder in Directory.GetDirectories("./Songs"))
        {
            Maps.Add(Map.ReadMaps(folder));
        }
        
        var random = new Random();
        int index = random.Next(Maps.Count);
        SelectedMap = Maps[index];
        Console.WriteLine(SelectedMap.SubMaps.Count);
        int subIndex = random.Next(SelectedMap.SubMaps.Count);
        SelectedSubMap = SelectedMap.SubMaps[subIndex];
        
        _songPlayer.Load($"{SelectedMap.FolderName}/{SelectedSubMap.AudioFile}");
        _songPlayer.SetVolume(0.8f);
        _songPlayer.Play();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _fontSystem = new();
        _fontSystem.AddFont(File.ReadAllBytes("./Fonts/arial.ttf"));
        font18 = _fontSystem.GetFont(18);

        _mainMenuSettingsTexture = Content.Load<Texture2D>("UI/SETTINGS");
        _mainMenuEditorTexture = Content.Load<Texture2D>("UI/EDITOR");
        _mainMenuPlayTexture = Content.Load<Texture2D>("UI/PLAY");
        _donSplashTexture = Content.Load<Texture2D>("UI/don-splash");

        _buttonClickSound = Content.Load<SoundEffect>("Audio/ButtonPressed");
        _buttonHoverSound = Content.Load<SoundEffect>("Audio/ButtonHover");

        _hitAreaCirleTexture = Primitives.CreateCircleWithOutline(GraphicsDevice, _hitObjectRadius + 5, 15);
    }

    protected override void Update(GameTime gameTime)
    {
        Input.GetKeyboardState();
        Input.GetMouseState();

        switch (_gameState)
        {
        case GameState.MainMenu:
        {
            _dontRotation += 0.05f * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            // Play
            if (Input.IsMouseHoveringOver(new Rectangle(10, Window.ClientBounds.Height - 10 - _mainMenuPlayTexture.Height - 10 - _mainMenuEditorTexture.Height - 10 - _mainMenuSettingsTexture.Height - 80, _mainMenuPlayTexture.Width, _mainMenuPlayTexture.Height), this))
            {
                _onHoverMovePlay = 30;
                if (!_isHoveringPlay)
                {
                    _buttonHoverSound.Play();
                    _isHoveringPlay = true;
                }
                if (Input.IsLeftMousePressed(true))
                {
                    _buttonClickSound.Play();
                }
            }
            // Editor
            else if (Input.IsMouseHoveringOver(new Rectangle(10, Window.ClientBounds.Height - 10 - _mainMenuPlayTexture.Height - 10 - _mainMenuEditorTexture.Height - 80, _mainMenuEditorTexture.Width, _mainMenuEditorTexture.Height), this))
            {
                _onHoverMoveEditor = 30;
                if (!_isHoveringPlay)
                {
                    _buttonHoverSound.Play();
                    _isHoveringPlay = true;
                }
                if (Input.IsLeftMousePressed(true))
                {
                    _buttonClickSound.Play();
                }
            }
            // Settings
            else if (Input.IsMouseHoveringOver(new Rectangle(10, Window.ClientBounds.Height - 10 - _mainMenuPlayTexture.Height - 80, _mainMenuSettingsTexture.Width, _mainMenuSettingsTexture.Height), this))
            {
                _onHoverMoveSettings = 30;
                if (!_isHoveringPlay)
                {
                    _buttonHoverSound.Play();
                    _isHoveringPlay = true;
                }
                if (Input.IsLeftMousePressed(true))
                {
                    _buttonClickSound.Play();
                }
            }
            else
            {
                _onHoverMovePlay = 0;
                _onHoverMoveEditor = 0;
                _onHoverMoveSettings = 0;
                _isHoveringPlay = false;
            }

            if (Input.IsKeyPressed(Keys.LeftControl, false))
            {
                if (Input.IsScrolled(Orientation.Down))
                {
                    _isVolumeChancing = true;
                    if (_songPlayer.GetVolume() - 0.05f < 0)
                    {
                        _songPlayer.SetVolume(0);
                    }
                    else
                    {
                        _songPlayer.SetVolume(_songPlayer.GetVolume()-0.05f);
                    }
                    
                }
                else if (Input.IsScrolled(Orientation.Up))
                {
                    _isVolumeChancing = true;
                    if (_songPlayer.GetVolume() + 0.05f > 1)
                    {
                        _songPlayer.SetVolume(1);
                    }
                    else
                    {
                        _songPlayer.SetVolume(_songPlayer.GetVolume()+0.05f);
                    }
                    
                }
                else
                {
                    _isVolumeChancing = false;
                }
            }
            
        }
            break;
        case GameState.SongSelect:
        {
            
        }
            break;
        case GameState.GamePlay:
        {
            if (_isPlaying)
            {
                _currentMapTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }
            break;
        case GameState.Editor:
        {
            
        }
            break;
        }


        Input.FixScrollLater();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        
        switch (_gameState)
        {
            case GameState.MainMenu:
            {
                _spriteBatch.Begin();
                // Draw menu buttons
                _spriteBatch.Draw(_mainMenuSettingsTexture, new Vector2(10+_onHoverMoveSettings, Window.ClientBounds.Height - 10 - _mainMenuPlayTexture.Height - 80), Color.White);
                _spriteBatch.Draw(_mainMenuEditorTexture, new Vector2(10+_onHoverMoveEditor, Window.ClientBounds.Height - 10 - _mainMenuPlayTexture.Height - 10 - _mainMenuEditorTexture.Height -80), Color.White);
                _spriteBatch.Draw(_mainMenuPlayTexture, new Vector2(10+_onHoverMovePlay, Window.ClientBounds.Height - 10 - _mainMenuPlayTexture.Height - 10 - _mainMenuEditorTexture.Height - 10 - _mainMenuSettingsTexture.Height - 80), Color.White);
                
                // Don splash rotating at the middle
                _spriteBatch.Draw(_donSplashTexture, new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2), null, Color.White, MathHelper.ToRadians(_dontRotation), new Vector2(_donSplashTexture.Width / 2, _donSplashTexture.Height / 2), 0.5f, SpriteEffects.None, 0f);

                if (_isVolumeChancing)
                {
                    var vol = _songPlayer.GetVolume() * 100;
                    _spriteBatch.DrawString(font18, $"Vol:{vol}", new(0, Window.ClientBounds.Height-20), Color.White);
                }
                
                _spriteBatch.End();
            }
                break;
            case GameState.SongSelect:
            {
            
            }
                break;
            case GameState.GamePlay:
            {
                _spriteBatch.Begin();
               
                _spriteBatch.DrawString(font18, "Song: " + _mapSong, new Vector2(10, 10), Color.White);
                _spriteBatch.DrawString(font18, "Artist: " + _mapArtist, new Vector2(10, 30), Color.White);
                _spriteBatch.DrawString(font18, "Creator: " + _mapCreator, new Vector2(10, 50), Color.White);
                _spriteBatch.DrawString(font18, "BPM: " + _currentMapBpm, new Vector2(10, 70), Color.White);
                _spriteBatch.DrawString(font18, "Audio: " + _mapAudioFile, new Vector2(10, 90), Color.White);
                _spriteBatch.DrawString(font18, "MapTime: " + _currentMapTime, new Vector2(10, 110), Color.White);
                _spriteBatch.DrawString(font18, "SongTime: " + _songPlayer.GetPosition(), new Vector2(10, 130), Color.White);

                _spriteBatch.Draw(
                    _hitAreaCirleTexture,
                    new Vector2(_hitObjectRadius + 5, Window.ClientBounds.Height / 2),
                    null, Color.White, 0f,
                    new Vector2((_hitObjectRadius + 5) / 2, (_hitObjectRadius + 5) / 2),
                    1f, SpriteEffects.None, 0f);

                _spriteBatch.End();
            }
                break;
            case GameState.Editor:
            {
            
            }
                break;
        }
        

        base.Draw(gameTime);
    }

    private void OnResize(object sender, EventArgs e)
    {
        if (Window.ClientBounds.Width < 800)
        {
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.ApplyChanges();
        }
        if (Window.ClientBounds.Height < 600)
        {
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();
        }
    }
}