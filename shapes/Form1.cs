using SkiaSharp;
using System.Numerics;

namespace shapes
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer mainTimer;

        private Cuboid Main;
        private FpsCounter Counter;
        private Camera MainCamera;
        private readonly static double CameraRotationIncrement = 2.5; 
        private readonly static float MovementIncrement = 5;

        public Form1()
        {
            InitializeComponent();

            mainTimer = new();
            mainTimer.Tick += new EventHandler(Tick);
            mainTimer.Interval = 40;
            mainTimer.Start();

            Counter = new();

            Main = new(new Vector3(100, 100, 100), 150);
            MainCamera = new();
        }


        private void Form1_Resize(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;

            this.pictureBox1.Size = new(control.Width, control.Height);
        }

        /// <summary>
        /// Draws an outline of the cuboid.
        /// </summary>
        /// <param name="canvas">Target canvas</param>
        private void DrawOutline(SKCanvas canvas)
        {
            var originLocal = MainCamera.CastToLocal(Main.Origin);
            var originOffset = originLocal - new Vector2(this.Width, this.Height)/2;
            
            using (SKPaint paint = new())
            {
                paint.Color = SKColors.DarkMagenta;
                paint.IsAntialias = true;
                paint.StrokeWidth = 5;
                paint.Style = SKPaintStyle.StrokeAndFill;

                foreach (var vertice in Main.Vertices.Values)
                {
                    var drawPos = MainCamera.CastToLocal(vertice) - originOffset;
                    canvas.DrawCircle(drawPos.X, drawPos.Y, 5, paint);
                }

                var a = MainCamera.CastToLocal(Main.Origin) - originOffset;
                canvas.DrawCircle(a.X, a.Y, 5, paint);
            }
        }

        /// <summary>
        /// Draw polygons of the cuboid.
        /// </summary>
        /// <param name="canvas">Target canvas</param>
        private void DrawPolygons(SKCanvas canvas)
        {
            canvas.Clear(SKColors.Black);

            var originOffset = MainCamera.CastToLocal(Main.Origin) - new Vector2(this.Width, this.Height) / 2;

            using (SKPaint paint = new())
            {
                paint.Color = SKColors.DarkMagenta;
                paint.IsAntialias = true;
                paint.StrokeWidth = 1;
                paint.Style = SKPaintStyle.StrokeAndFill;

                for (int i=0; i<12; i++)
                {
                    var polygon = Main.Polygons[i];
                    var localPoints = MainCamera.CastArrayToLocal(polygon.Vertices.ToArray(), originOffset);

                    Byte val = 255;
                    var dot = Vector3.Dot(polygon.SurfaceVector, MainCamera.CameraVector);

                    if (dot <= 0) continue;
                    val = (Byte)(val * Math.Pow(dot, 1d / 4d));

                    var newPaint = paint.Clone();
                    newPaint.Color = new SKColor(val, val, val);
                    var path = new SKPath { FillType = SKPathFillType.EvenOdd };
                    path.MoveTo(localPoints[0]);

                    for (int j = 0; j < 3; j++)
                        path.LineTo(localPoints[(j + 1) % 3]);

                    path.Close();
                    canvas.DrawPath(path, newPaint);
                }
            }
        }

        /// <summary>
        /// Draw the cuboid on the screen.
        /// </summary>
        private void Draw()
        {
            SKImageInfo imageInfo = new(this.Width, this.Height);

            using SKSurface surface = SKSurface.Create(imageInfo);
            {
                SKCanvas canvas = surface.Canvas;

                DrawPolygons(canvas);
                //DrawOutline(canvas);

                using (SKImage image = surface.Snapshot())
                using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (MemoryStream mStream = new(data.ToArray()))
                {
                    Bitmap bm = new(mStream, false);
                    pictureBox1.Image = bm;
                }

            }
        }

        /// <summary>
        /// Handles the keyboard controls.
        /// </summary>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    MainCamera.AddToAzimuth(CameraRotationIncrement);
                    break;
                case Keys.Right:
                    MainCamera.AddToAzimuth(-CameraRotationIncrement);
                    break;
                case Keys.Up:
                    MainCamera.ModifyElevation(CameraRotationIncrement);
                    break;
                case Keys.Down:
                    MainCamera.ModifyElevation(-CameraRotationIncrement);
                    break;
                case Keys.A:
                    MainCamera.ModifyOrigin(new(-MovementIncrement, 0, 0));
                    break;
                case Keys.D:
                    MainCamera.ModifyOrigin(new(MovementIncrement, 0, 0));
                    break;
                case Keys.W:
                    MainCamera.ModifyOrigin(new(0, MovementIncrement, 0));
                    break;
                case Keys.S:
                    MainCamera.ModifyOrigin(new(0, -MovementIncrement, 0));
                    break;
                case Keys.Z:
                    MainCamera.ModifyOrigin(new(0, 0, MovementIncrement));
                    break;
                case Keys.X:
                    MainCamera.ModifyOrigin(new(0, 0, -MovementIncrement));
                    break;
                case Keys.R:
                    MainCamera.CameraOrigin = Vector3.Zero;
                    MainCamera.SetAzimuth(0);
                    MainCamera.SetElevation(90);
                    break;
            }

        }

    }

}