﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace MetroRadiance.Interop.Win32
{
	public static class User32
	{
		[DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		public static WindowStyles GetWindowLong(IntPtr hWnd)
		{
			return (WindowStyles)GetWindowLong(hWnd, (int)WindowLongFlags.GWL_STYLE);
		}
		public static WindowExStyles GetWindowLongEx(IntPtr hWnd)
		{
			return (WindowExStyles)GetWindowLong(hWnd, (int)WindowLongFlags.GWL_EXSTYLE);
		}

		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		public static WindowStyles SetWindowLong(IntPtr hWnd, WindowStyles dwNewLong)
		{
			return (WindowStyles)SetWindowLong(hWnd, (int)WindowLongFlags.GWL_STYLE, (int)dwNewLong);
		}
		public static WindowExStyles SetWindowLongEx(IntPtr hWnd, WindowExStyles dwNewLong)
		{
			return (WindowExStyles)SetWindowLong(hWnd, (int)WindowLongFlags.GWL_EXSTYLE, (int)dwNewLong);
		}

		[DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool _SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags flags);

		public static void SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags flags)
		{
			var ret = _SetWindowPos(hWnd, hWndInsertAfter, x, y, cx, cy, flags);
			if (!ret) throw new Win32Exception(Marshal.GetLastWin32Error());
		}

		[DllImport("user32.dll")]
		public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

		[DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

		public static WINDOWPLACEMENT GetWindowPlacement(IntPtr hWnd)
		{
			WINDOWPLACEMENT wndpl;
			if (!GetWindowPlacement(hWnd, out wndpl)) 
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			return wndpl;
		}

		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd, out RECT rect);

		[DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

		public static RECT GetWindowRect(IntPtr hWnd)
		{
			RECT rect;
			var ret = GetWindowRect(hWnd, out rect);
			if (!ret) throw new Win32Exception(Marshal.GetLastWin32Error());
			return rect;
		}

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		private static extern bool PostMessageW(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		public static void PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			var ret = PostMessageW(hWnd, msg, wParam, lParam);
			if (!ret) throw new Win32Exception(Marshal.GetLastWin32Error());
		}

		[DllImport("user32.dll", EntryPoint = "SendMessageW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		public static extern IntPtr SendMessage(IntPtr hWnd, WindowsMessages msg, IntPtr wParam, IntPtr lParam);

		public static WindowClassStyles GetClassLong(IntPtr hwnd, ClassLongPtrIndex nIndex)
		{
			return Environment.Is64BitProcess
				? (WindowClassStyles)GetClassLong64(hwnd, nIndex)
				: (WindowClassStyles)GetClassLong32(hwnd, nIndex);
		}

		[DllImport("user32.dll", EntryPoint = "GetClassLong")]
		public static extern IntPtr GetClassLong32(IntPtr hwnd, ClassLongPtrIndex nIndex);

		[DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
		public static extern IntPtr GetClassLong64(IntPtr hwnd, ClassLongPtrIndex nIndex);

		public static WindowClassStyles SetClassLong(IntPtr hwnd, ClassLongPtrIndex nIndex, WindowClassStyles dwNewLong)
		{
			return Environment.Is64BitProcess
				? (WindowClassStyles)SetClassLong64(hwnd, nIndex, (IntPtr)dwNewLong)
				: (WindowClassStyles)SetClassLong32(hwnd, nIndex, (IntPtr)dwNewLong);
		}

		[DllImport("user32.dll", EntryPoint = "SetClassLong")]
		public static extern IntPtr SetClassLong32(IntPtr hWnd, ClassLongPtrIndex nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetClassLongPtr")]
		public static extern IntPtr SetClassLong64(IntPtr hWnd, ClassLongPtrIndex nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", EntryPoint = "GetMonitorInfoW", SetLastError = true, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

		[DllImport("user32.dll", EntryPoint = "GetMonitorInfoW", SetLastError = true, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetMonitorInfoEx(IntPtr hMonitor, ref MONITORINFOEX lpmi);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr MonitorFromWindow(IntPtr hwnd, MonitorDefaultTo dwFlags);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern IntPtr GetActiveWindow();

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		public static extern IntPtr SetActiveWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool IsZoomed(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool IsIconic(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndNewParent);

		[DllImport("user32.dll")]
		public static extern IntPtr GetParent(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool CloseWindow(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetWindowCompositionAttribute(IntPtr hWnd, ref WindowCompositionAttributeData data);

		public static void SetWindowCompositionAttribute(IntPtr hWnd, WindowCompositionAttributeData data)
		{
			var ret = SetWindowCompositionAttribute(hWnd, ref data);
			if (!ret) throw new Win32Exception(Marshal.GetLastWin32Error());
		}
	}
}
