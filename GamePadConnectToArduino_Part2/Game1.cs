using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO.Ports;
using System.Threading.Tasks;

namespace GamePadConnectToArduino_Part2
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SerialPort _serialPort;

        public Game1()
        {
            this._graphics = new GraphicsDeviceManager(this);

            // Verwendete COM Port muss ggf. angepasst werden.
            this._serialPort = new SerialPort("COM15", 115200, Parity.None, 8, StopBits.One);
            this._serialPort.Open();
        }

        /// <summary>
        /// Close the serial port connection
        /// </summary>
        protected override void UnloadContent()
        {
            this._serialPort.DiscardOutBuffer();
            this._serialPort.Close();
        }

        /// <summary>
        /// Update the state from xbox controller.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            GamePadState state = GamePad.GetState(PlayerIndex.One);

            if (state.Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
            
            if (this._serialPort.BytesToWrite == 0)
            {
                byte stickValueX = (byte)((state.ThumbSticks.Left.X + 1) * 90);
                byte stickValueY = (byte)((state.ThumbSticks.Left.Y + 1) * 90);
                byte buttonValuePaint = (byte)(state.Buttons.A == ButtonState.Pressed ? 10 : 0);
                byte buttonValueReset = (byte)(state.Buttons.B == ButtonState.Pressed ? 10 : 0);
                this._serialPort.Write(new byte[] { stickValueX, stickValueY, buttonValuePaint, buttonValueReset }, 0, 4);
                this._serialPort.WriteLine(string.Empty);
            }

            Task.Delay(5);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}
