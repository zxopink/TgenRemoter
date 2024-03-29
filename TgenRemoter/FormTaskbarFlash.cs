﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

public static class FormTaskbarFlash
{
    // To support flashing.
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

    //Flash both the window caption and taskbar button.
    //This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags. 
    public const UInt32 FLASHW_ALL = 3;
    //Flash the taskbar button.
    public const UInt32 FLASHW_TRAY = 2;

    // Flash continuously until the window comes to the foreground. 
    public const UInt32 FLASHW_TIMERNOFG = 12;

    [StructLayout(LayoutKind.Sequential)]
    public struct FLASHWINFO
    {
        public UInt32 cbSize;
        public IntPtr hwnd;
        public UInt32 dwFlags;
        public UInt32 uCount;
        public UInt32 dwTimeout;
    }

    public static bool FlashWindowEx(Form form)
    {
        IntPtr hWnd = form.Handle;
        FLASHWINFO fInfo = new FLASHWINFO();

        fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
        fInfo.hwnd = hWnd;
        fInfo.dwFlags = FLASHW_TRAY;
        fInfo.uCount = 3;
        fInfo.dwTimeout = 2;

        return FlashWindowEx(ref fInfo);
    }

    /*Old default flashing, we want a different effect for files
    // Do the flashing - this does not involve a raincoat.
    public static bool FlashWindowEx(Form form)
    {
        IntPtr hWnd = form.Handle;
        FLASHWINFO fInfo = new FLASHWINFO();

        fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
        fInfo.hwnd = hWnd;
        fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
        fInfo.uCount = UInt32.MaxValue;
        fInfo.dwTimeout = 0;

        return FlashWindowEx(ref fInfo);
    }
    */
}