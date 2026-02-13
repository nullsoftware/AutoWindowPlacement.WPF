using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace NullSoftware.Windows
{
    /// <summary>
    /// A static class that provides methods to get and set window placement for WPF windows using the Windows API.
    /// </summary>
    public static class WindowPlacementManager
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}, {3}", Left, Top, Right, Bottom);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }

            public override string ToString()
            {
                return string.Format("{0}, {1}", X, Y);
            }
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINT minPosition;
            public POINT maxPosition;
            public RECT normalPosition;
        }
#pragma warning restore CS1591 // Restore the warning for the rest of the file

        [DllImport("user32.dll")]
        private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;

        /// <summary>
        /// Sets the window placement for the specified window. 
        /// If the window is minimized, it will be restored to normal state before applying the placement.
        /// </summary>
        /// <param name="window">Window to set placement for.</param>
        /// <param name="placement">The desired window placement.</param>
        public static void SetPlacement(Window window, WINDOWPLACEMENT placement)
        {
            placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
            placement.flags = 0;
            placement.showCmd = (placement.showCmd == SW_SHOWMINIMIZED ? SW_SHOWNORMAL : placement.showCmd);
            SetWindowPlacement(new WindowInteropHelper(window).Handle, ref placement);
        }

        /// <summary>
        /// Gets the current window placement for the specified window.
        /// </summary>
        /// <param name="window">Window to get placement for.</param>
        /// <returns>The current window placement.</returns>
        public static WINDOWPLACEMENT GetPlacement(Window window)
        {
            WINDOWPLACEMENT wp;
            GetWindowPlacement(new WindowInteropHelper(window).Handle, out wp);

            return wp;
        }

        /// <summary>
        /// Serializes the <see cref="WINDOWPLACEMENT"/> structure to a byte array. 
        /// This can be useful for saving the window placement to a file or database.
        /// </summary>
        /// <param name="placement">The window placement to serialize.</param>
        /// <returns>A byte array representing the serialized window placement.</returns>
        public static byte[] Serialize(WINDOWPLACEMENT placement)
        {
            int size = Marshal.SizeOf(placement);
            byte[] result = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(placement, ptr, true);
            Marshal.Copy(ptr, result, 0, size);
            Marshal.FreeHGlobal(ptr);

            return result;
        }

        /// <summary>
        /// Deserializes a byte array back into a <see cref="WINDOWPLACEMENT"/> structure.
        /// </summary>
        /// <param name="data">The byte array to deserialize.</param>
        /// <returns>The deserialized <see cref="WINDOWPLACEMENT"/> structure.</returns>
        public static WINDOWPLACEMENT Deserialize(byte[] data)
        {
            WINDOWPLACEMENT wp = new WINDOWPLACEMENT();

            int size = Marshal.SizeOf(wp);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(data, 0, ptr, size);

            wp = (WINDOWPLACEMENT)Marshal.PtrToStructure(ptr, wp.GetType())!;
            Marshal.FreeHGlobal(ptr);

            return wp;
        }
    }
}