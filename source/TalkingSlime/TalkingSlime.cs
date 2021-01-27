/* ----------------------------------------------------------------------------
    MIT License

    Copyright (c) 2020 Christopher Whitley

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
---------------------------------------------------------------------------- */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections;
using System.Diagnostics;

namespace TalkingSlime
{
    public class TalkingSlime : Game
    {
        // --------------------------------------------------------------------
        //
        //  The following values can/need to be edited based on your system setup.
        //
        //  _microphoneName:    This is the name of your microphone listening device to use
        //                      for detecting audio input.  It can be a partial word.
        //
        //  _micDeviceID:       This is the device ID of your microphone.  You can find
        //                      this value by running this game once within visual studio
        //                      then checking the Output panel window which will show
        //                      all detected input devices and their ids.
        //
        //  _closeThreshold:    This is a float value between 0.0f and 100.0f.  This
        //                      value determines at which point in the microphone levels
        //                      it is determined that the microphone is closed.  To
        //                      find your appropriate level, run the game and look at the
        //                      window title bar to see the current levels and choose based
        //                      on that information.
        //
        //  _openThreshold:     This is a float value between 0.0f and 100.0f.  This value
        //                      determines at which point in the microphone's levels it is
        //                      determined that the microphone is open.  To find your appropriate
        //                      level, run the game and look at the window title bar to see the
        //                      current levels and choose based on that information.
        //
        //  _defaultScale:      This is the default x and y axis scale value to use when rendering
        //                      the mouth. This value is used when the microphone is in a "closed"
        //                      state.
        //
        //  _scaleMin:          This is the minimum x and y axis scale value to use when rendering
        //                      the mouth only during the microphone "open" state.
        //
        //  _scaleMax:          This is the maximum x and y axis scale value to use when rendering
        //                      the mouth only during the microphone "open" state.
        // --------------------------------------------------------------------


        //  The name of your microphone. This can be a partial word value. 
        private string _microphoneName = "Logitech";

        //  This is the index of your microphone as found by the NAudio library.
        //  To determine this value, after running the applicaiton once, check the
        //  output window in VisualStudio where Debug messages are written to get
        //  the appropriate values for your microphone.
        private int _micDeviceId = 0;

        //  This value determines at what level the microphone must reach
        //  to determine that it is closed (no talking detected).
        private float _closeThreshold = 0.5f;

        //  This value determines at what level the microphone must reach
        //  to determine that it is open (talking detected).
        private float _openThreshold = 7.0f;

        //  The default scale value to use for the mouth when no talking is occurring.
        private float _defaultScale = 0.25f;

        //  The minimum value that can be used for the mouth scale when talking is detected.
        private float _scaleMin = 1.0f;

        //  The maximum value that can be used for the mouth scale when talking is detected.
        private float _scaleMax = 1.25f;


        // --------------------------------------------------------------------
        //
        //
        //  The fields after this point don't need any adjustments unless you
        //  plan to customize the textures that are used in the game. 
        //
        //
        // --------------------------------------------------------------------

        //  The graphics device manager used to manage the presentation of graphics.
        private GraphicsDeviceManager _graphics;

        //  The SpriteBatch instnace used for 2D rendering.
        private SpriteBatch _spriteBatch;

        //  The microphone device used to get microphon audio.
        private MMDevice _mic;

        //  The texture used to render the slime's body
        private Texture2D _slime;

        //  The texture used to render the mouth of the slime.
        private Texture2D _mouth;

        //  The texture used to render the eyes of the slime.
        private Texture2D _eye;

        //  The xy-coordinate position of the left eye
        private Vector2 _leftEyePosition;

        //  The xy-coordiatne origin point of the left eye.
        private Vector2 _leftEyeOrigin;

        //  The xy-coordinate position of the right eye.
        private Vector2 _rightEyePosition;

        //  The xy-coordinate origin point of the right eye.
        private Vector2 _rightEyeOrigin;

        //  Stores the value used to scale the left and right eyes on the y-axis.
        //  This is used to create the blinking effect.
        private float _eyeScaleY;

        //  The xy-coordinate origin point of the mouth
        private Vector2 _mouthOrigin;

        //  The xy-coordinate position of the mouth.
        private Vector2 _mouthPosition;

        //  Stores the value used to scale the mouth when talking is detected.
        private float _mouthScale;

        //  Indicates if the microphone is open (talking detected).
        private bool _isOpen;

        //  The current microphone peak level detected.
        private float _currentLevel;

        //  Used to track the amount of time (in seconds) that the mic has
        //  been open (talking detected).
        private float _openTime;

        //  Instance used to start microphone listening.
        WaveInEvent _waveIn;

        //  IEnumerator used to blink the eyes.
        private IEnumerator _blinkEyes;

        //  A collection of values that are randomly picked from to deteremine the amount of time
        //  to wait before the next blink event should occur.
        private double[] _blinkChance = new double[] { 5, 10, 2, 7, 8 };

        //  Used to track the amount of time to wait for hte next blink event.
        private TimeSpan _timeUntilBlink;

        //  Used to generate a random value.
        private Random _random = new Random();

        //  The amount of time that has passed between the current and previous
        //  update frames.
        private TimeSpan _deltaTime;

        /// <summary>
        ///     Creates a new <see cref="TalkingSlime"/> instance.
        /// </summary>
        public TalkingSlime()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        ///     Initializes the game. Called internally by the 
        ///     MonoGame Framework.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            //  The backbuffer widht and height are set to the size of the sprite so the window is
            //  only as big as needs to be.
            _graphics.PreferredBackBufferWidth = _slime.Width;
            _graphics.PreferredBackBufferHeight = _slime.Height;
            _graphics.ApplyChanges();


            //  Create the wave in device event.
            _waveIn = new WaveInEvent();

            //  output the device numbers to the output/debug window.
            int waveInDeviceCount = WaveInEvent.DeviceCount;
            Debug.WriteLine($"{waveInDeviceCount} Devices Detected");
            Debug.WriteLine($"Find the device ID of your mic below and use it as the value for the value of _micDeviceId");
            for(int i = 0; i < waveInDeviceCount; i++)
            {
                WaveInCapabilities capabilities = WaveInEvent.GetCapabilities(i);
                Debug.WriteLine($"Device ID: {i} | Name: {capabilities.ProductName}");
            }

            _waveIn.DeviceNumber = _micDeviceId;

            //  Start listening for the mic.
            _waveIn.StartRecording();

            //  Find the microphone.  For me, it's my Logitech headset.
            MMDeviceEnumerator enm = new MMDeviceEnumerator();
            var devices = enm.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            foreach (var device in devices)
            {
                if (device.FriendlyName.Contains(_microphoneName))
                {
                    _mic = device;
                    break;
                }
            }

            //  Set the position/origin/scale of the mouth.
            _mouthPosition = new Vector2(507.0f, 440.0f);
            _mouthOrigin = new Vector2(_mouth.Width, _mouth.Height) * 0.5f;
            _mouthScale = _defaultScale;

            //  Set the position/origin of the left eye
            _leftEyePosition = new Vector2(446.0f, 244.0f);
            _leftEyeOrigin = new Vector2(_eye.Width, _eye.Height) * 0.5f;

            //  Set the position/origin of the right eye.
            _rightEyePosition = new Vector2(570.0f, 244.0f);
            _rightEyeOrigin = new Vector2(_eye.Width, _eye.Height) * 0.5f;
            _eyeScaleY = 1.0f;

            //  Determine the initial amount of time to wait before blinking.
            _timeUntilBlink = TimeSpan.FromSeconds(_blinkChance[_random.Next(_blinkChance.Length)]);
        }

        /// <summary>
        ///     Loads the graphical assets for the application. This
        ///     is called interanlly by the MonoGame framework during the 
        ///     <c>base.Initialize()</c> call.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //  Load the textures.
            _slime = Content.Load<Texture2D>("slime");
            _mouth = Content.Load<Texture2D>("mouth");
            _eye = Content.Load<Texture2D>("eye");

        }

        /// <summary>
        ///     Updates the game.
        /// </summary>
        /// <param name="gameTime">
        ///     A snapshot of the timing values provided by the MonoGame framework.
        /// </param>
        protected override void Update(GameTime gameTime)
        {
            //  Cache the elapsed time for this frame.
            _deltaTime = gameTime.ElapsedGameTime;

            //  Update the logic for blinking the eyes.
            UpdateBlinking();

            //  Update the logic for the mic input level and mouth moving.
            UpdateMicLevel();


            base.Update(gameTime);
        }

        /// <summary>
        ///     Handles the update logic each frame for blinking the
        ///     eyes of the slime.
        /// </summary>
        private void UpdateBlinking()
        {
            //  Decrease the time until next blink by the delta time.
            _timeUntilBlink -= _deltaTime;

            //  If the eyes are not blinking, and the time to wait is now below
            //  zero, then blink the eyes.
            if (_blinkEyes == null && _timeUntilBlink <= TimeSpan.Zero)
            {
                _blinkEyes = BlinkEyes();
            }

            //  If the eyes are blinking, but we are finished with blinking
            //  the stop blinking and set the amount of time to wait until blinking again.
            if (_blinkEyes != null && !_blinkEyes.MoveNext())
            {
                _blinkEyes = null;
                _timeUntilBlink = TimeSpan.FromSeconds(_blinkChance[_random.Next(_blinkChance.Length)]);
            }
        }

        /// <summary>
        ///     Handles the update logic each frame for reading the microphone
        ///     level and animating the mouth when talking.
        /// </summary>
        private void UpdateMicLevel()
        {
            _currentLevel = _mic.AudioMeterInformation.MasterPeakValue * 100.0f;

            //  If the mic is currently NOT open and the current level of 
            //  the mic has exceeded the open threshold, then open the mic.
            //  Otherwise if the mic IS open and the current level of the mic
            //  has decreased below the close threshold, close the mic.
            if (!_isOpen && _currentLevel > _openThreshold)
            {
                _isOpen = true;
            }
            else if (_isOpen && _currentLevel < _closeThreshold)
            {
                _isOpen = false;
            }

            //  If the mic is open, then we scale the mouth in and out.
            //  Otherwise, with the mic being closed, we scale the mouth
            //  down to default scale.
            if (_isOpen)
            {
                _openTime += (float)_deltaTime.TotalSeconds;
                float t = (float)Math.Cos(_openTime * (MathHelper.PiOver2 * 10));
                _mouthScale = MathHelper.LerpPrecise(_scaleMin, _scaleMax, t);

            }
            else
            {
                _openTime = 0.0f;
                _mouthScale = Approach(_mouthScale, _defaultScale, 0.05f);
            }
        }

        /// <summary>
        ///     An IEnumerator function used to blink the eyes.
        /// </summary>
        /// <returns></returns>
        private IEnumerator BlinkEyes()
        {
            //  The total amount of time, in seconds, to animate the eyes
            //  closed. Also used as the total amount of time, in seconds,
            //  to animate the eyes open.
            TimeSpan totalTime = TimeSpan.FromSeconds(0.1f);

            //  The timer used to track the amount of time spent animating.
            TimeSpan timer = totalTime;

            //  Close the eyes
            while (timer > TimeSpan.Zero)
            {
                timer -= _deltaTime;
                _eyeScaleY = MathHelper.LerpPrecise(1.0f, 0.0f, 1.0f - (float)(timer / totalTime));
                yield return null;
            }

            //  Rest the time
            timer = totalTime;

            //  Open the eyes.
            while (timer > TimeSpan.Zero)
            {
                timer -= _deltaTime;
                _eyeScaleY = MathHelper.LerpPrecise(0.0f, 1.0f, 1.0f - (float)(timer / totalTime));
                yield return null;
            }
        }

        /// <summary>
        ///     Renders everything to the screen.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            //  Clear the graphics device using the default green screen value.
            GraphicsDevice.Clear(new Color(0, 177, 64));

            //  Update the window title bar with debug information.
            Window.Title = $"Threshold: {_currentLevel} | Close: {_closeThreshold} | Open: {_openThreshold}";

            //  Draw everything
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
            _spriteBatch.Draw(_slime, Vector2.Zero, Color.White);
            _spriteBatch.Draw(_mouth, _mouthPosition, null, Color.White, 0.0f, _mouthOrigin, _mouthScale, SpriteEffects.None, 0.0f);
            _spriteBatch.Draw(_eye, _leftEyePosition, null, Color.White, 0.0f, _leftEyeOrigin, new Vector2(1.0f, _eyeScaleY), SpriteEffects.None, 0.0f);
            _spriteBatch.Draw(_eye, _rightEyePosition, null, Color.White, 0.0f, _rightEyeOrigin, new Vector2(1.0f, _eyeScaleY), SpriteEffects.None, 0.0f);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        ///     Moves the <paramref name="value"/> given towards the <paramref name="target"/>
        ///     value by the <paramref name="step"/> amount.
        /// </summary>
        /// <param name="value">
        ///     A <see cref="float"/> value that defines the initial value to use.
        /// </param>
        /// <param name="target">
        ///     A <see cref="float"/> value that defines the value to approach.
        /// </param>
        /// <param name="step">
        ///     A <see cref="float"/> value that defines the amount to move.
        /// </param>
        private float Approach(float value, float target, float step)
        {
            if (value > target)
            {
                return Math.Max(value - step, target);
            }
            else
            {
                return Math.Min(value + step, target);
            }
        }
    }
}
