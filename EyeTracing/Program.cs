using OpenCvSharp;
using System.Runtime.InteropServices;

var screen = Utils.WorkingArea;
int stay = 10;
int speed = 10;
SetCursorPos(screen.Width / 2, screen.Height / 2);
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
            SetCursorPos(Cursor_x, Cursor_y);
            //SetCursorPos(face_x * screen.Width / image.Width, face_y * screen.Height / screen.Height);
            image.Rectangle(face_match[0], Scalar.Red, 3);
            var face = new Mat(image_g, face_match[0]);
            haarCascade = new CascadeClassifier("haarcascade_eye_tree_eyeglasses.xml");
            Rect[] eyes = haarCascade.DetectMultiScale(
                                face, 1.2, 2, HaarDetectionTypes.ScaleImage, new Size(30, 30));
            if (eyes.Length == 1 && !LeftDown)
            {
                //左键
                mouse_event(MouseEventFlag.LeftDown, Cursor_x, Cursor_y, 0, UIntPtr.Zero);
                LeftDown = true;
            }
            if (eyes.Length == 2)
            {
                //左键抬起
                if (LeftDown)
                {
                    LeftDown = false;
                    mouse_event(MouseEventFlag.LeftUp, Cursor_x, Cursor_y, 0, UIntPtr.Zero);
                }
                //右键抬起
                if (RightDown)
                {
                    RightDown = false;
                    mouse_event(MouseEventFlag.RightUp, Cursor_x, Cursor_y, 0, UIntPtr.Zero);
                }
            }
            if (eyes.Length == 0&&!RightDown)
            {
                RightDown = true;
                mouse_event(MouseEventFlag.RightDown, Cursor_x, Cursor_y, 0, UIntPtr.Zero);
            }
            //head.ShowImage(face);
        }
        //window.ShowImage(image);
        Cv2.WaitKey(30);
    }
}

[DllImport("user32.dll")]
static extern bool SetCursorPos(int X, int Y);
[DllImport("user32.dll")]
static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);
[Flags]
enum MouseEventFlag : uint
{
    Move = 0x0001,
    LeftDown = 0x0002,
    LeftUp = 0x0004,
    RightDown = 0x0008,
    RightUp = 0x0010,
    MiddleDown = 0x0020,
    MiddleUp = 0x0040,
    XDown = 0x0080,
    XUp = 0x0100,
    Wheel = 0x0800,
    VirtualDesk = 0x4000,
    Absolute = 0x8000
}
class Utils
{
    #region Win32 API
    [DllImport("user32.dll")]
    static extern IntPtr GetDC(IntPtr ptr);
    [DllImport("gdi32.dll")]
    static extern int GetDeviceCaps(
    IntPtr hdc, // handle to DC
    int nIndex // index of capability
    );
    [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
    static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
    #endregion
    #region DeviceCaps常量
    const int HORZRES = 8;
    const int VERTRES = 10;
    const int LOGPIXELSX = 88;
    const int LOGPIXELSY = 90;
    const int DESKTOPVERTRES = 117;
    const int DESKTOPHORZRES = 118;
    #endregion
    public static Size WorkingArea
    {
        get
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            Size size = new Size();
            size.Width = GetDeviceCaps(hdc, HORZRES);
            size.Height = GetDeviceCaps(hdc, VERTRES);
            ReleaseDC(IntPtr.Zero, hdc);
            return size;
        }
    }
}