using OpenCvSharp;
using System.Runtime.InteropServices;
namespace EyeTracing
{
    public class Program
    {
        public delegate void Fuck(string who);
        public static event Fuck Fucking;
        public static void Main(string[] args)
        {
            Fucking += (who) => { Console.WriteLine("xml is fucked by " + who); };
            Fucking("A dog");
            return;
            var screen = Utils.WorkingArea;
            int stay = 10;
            int speed = 10;
            Utils.SetCursorPos(screen.Width / 2, screen.Height / 2);
            var Cursor_x = screen.Width / 2;
            var Cursor_y = screen.Height / 2;
            bool LeftDown = false;
            bool RightDown = false;
            VideoCapture capture = new VideoCapture(0);
            //using(Window head=new Window("Head"))
            //using (Window window = new Window("Camera"))
            using (Mat image = new Mat()) // Frame image buffer
            {
                // When the movie playback reaches end, Mat.data becomes NULL.
                while (true)
                {
                    GC.Collect();
                    capture.Read(image); // same as cvQueryFrame
                    if (image.Empty()) break;

                    var haarCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
                    using var image_g = image.CvtColor(ColorConversionCodes.BGR2GRAY);
                    var face_match = haarCascade.DetectMultiScale(image_g);
                    if (face_match.Any())
                    {
                        var face_pos = face_match[0];
                        var face_x = face_pos.X + face_pos.Width / 2;
                        var face_y = face_pos.Y + face_pos.Height / 2;
                        ///###
                        ///#+#
                        ///###
                        if (face_x - image.Width / 2 > stay)
                        {
                            Cursor_x -= speed;
                        }
                        if (face_x - image.Width / 2 < -stay)
                        {
                            Cursor_x += speed;
                        }
                        if (face_y - image.Height / 2 > stay)
                        {
                            Cursor_y += speed;
                        }
                        if (face_y - image.Height / 2 < -stay)
                        {
                            Cursor_y -= speed;
                        }
                        Utils.SetCursorPos(Cursor_x, Cursor_y);
                        image.Rectangle(face_match[0], Scalar.Red, 3);
                        var face = new Mat(image_g, face_match[0]);
                        haarCascade = new CascadeClassifier("haarcascade_eye_tree_eyeglasses.xml");
                        Rect[] eyes = haarCascade.DetectMultiScale(
                                            face, 1.2, 2, HaarDetectionTypes.ScaleImage, new Size(30, 30));
                        if (eyes.Length == 1 && !LeftDown)
                        {
                            //左键
                            Utils.mouse_event(MouseEventFlag.LeftDown, Cursor_x, Cursor_y, 0, UIntPtr.Zero);
                            LeftDown = true;
                        }
                        if (eyes.Length == 2)
                        {
                            //左键抬起
                            if (LeftDown)
                            {
                                LeftDown = false;
                                Utils.mouse_event(MouseEventFlag.LeftUp, Cursor_x, Cursor_y, 0, UIntPtr.Zero);
                            }
                            //右键抬起
                            if (RightDown)
                            {
                                RightDown = false;
                                Utils.mouse_event(MouseEventFlag.RightUp, Cursor_x, Cursor_y, 0, UIntPtr.Zero);
                            }
                        }
                        if (eyes.Length == 0 && !RightDown)
                        {
                            RightDown = true;
                            Utils.mouse_event(MouseEventFlag.RightDown, Cursor_x, Cursor_y, 0, UIntPtr.Zero);
                        }
                        //head.ShowImage(face);
                    }
                    //window.ShowImage(image);
                    Cv2.WaitKey(30);
                }
            }
        }
    }
}
