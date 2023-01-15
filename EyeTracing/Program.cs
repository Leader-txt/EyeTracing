using OpenCvSharp;
using System.Runtime.InteropServices;
namespace EyeTracing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var screen = Utils.WorkingArea;
            int stay = 10;
            int speed = 10;
            Utils.SetCursorPos(screen.Width / 2, screen.Height / 2);
            var Cursor_x = screen.Width / 2;
            var Cursor_y = screen.Height / 2;
            bool LeftDown = false;
            bool RightDown = false;
            var faceRectQue = new Queue<Rect>(5);
            VideoCapture capture = new VideoCapture(0);
            using (Window head = new Window("Head"))
            using (Window window = new Window("Camera"))
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
                        faceRectQue.Enqueue(face_match[0]);
                        if (faceRectQue.Count > 5)
                        {
                            faceRectQue.Dequeue();
                        }
                        var faceRect = new Rect(faceRectQue.Select(x => x.X).Sum() / faceRectQue.Count, faceRectQue.Select(x => x.Y).Sum() / faceRectQue.Count
                            , faceRectQue.Select(x => x.Width).Sum() / faceRectQue.Count, faceRectQue.Select(x => x.Height).Sum() / faceRectQue.Count);
                        var face_x = faceRect.X + faceRect.Width / 2;
                        var face_y = faceRect.Y + faceRect.Height / 2;
                        ///###
                        ///#+#
                        ///###
                        /*if (face_x - image.Width / 2 > stay)
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
                        Utils.SetCursorPos(Cursor_x, Cursor_y);*/
                        int curx = screen.Width / 2 - speed * (face_x - image.Width / 2);
                        int cury = screen.Height / 2 + speed * (face_y - image.Height / 2);
                        if (curx > Cursor_x)
                        {
                            Cursor_x += 5;
                        }
                        else if (curx < Cursor_x)
                        {
                            Cursor_x -= 5;
                        }
                        if (cury > Cursor_y)
                        {
                            Cursor_y += 5;
                        }
                        else if (cury < Cursor_y)
                        {
                            Cursor_y -= 5;
                        }
                        //Console.WriteLine(curx+","+ cury);
                        Utils.SetCursorPos(Cursor_x, Cursor_y);
                        image.Rectangle(faceRect, Scalar.Red, 3);
                        var face = new Mat(image_g, faceRect);
                        haarCascade = new CascadeClassifier("haarcascade_eye_tree_eyeglasses.xml");
                        Rect[] eyes = haarCascade.DetectMultiScale(face);
                        foreach (var rect in eyes)
                        {
                            face.Rectangle(rect, Scalar.Red, 3);
                        }
                        if (eyes.Length == 1 && !LeftDown)
                        {
                            //左键
                            //Utils.mouse_event(MouseEventFlag.LeftDown, Cursor_x, Cursor_y, 0, UIntPtr.Zero);
                            LeftDown = true;
                        }
                        if (eyes.Length == 2)
                        {
                            //左键抬起
                            if (LeftDown)
                            {
                                LeftDown = false;
                                //Utils.mouse_event(MouseEventFlag.LeftUp, Cursor_x, Cursor_y, 0, UIntPtr.Zero);
                            }
                            //右键抬起
                            if (RightDown)
                            {
                                RightDown = false;
                                //Utils.mouse_event(MouseEventFlag.RightUp, Cursor_x, Cursor_y, 0, UIntPtr.Zero);
                            }
                        }
                        if (eyes.Length == 0 && !RightDown)
                        {
                            RightDown = true;
                            //Utils.mouse_event(MouseEventFlag.RightDown, Cursor_x, Cursor_y, 0, UIntPtr.Zero);
                        }
                        head.ShowImage(face);
                    }
                    window.ShowImage(image);
                    Cv2.WaitKey(30);
                }
            }
        }
    }
}
