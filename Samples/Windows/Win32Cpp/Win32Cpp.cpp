// Win32Cpp.cpp : Defines the entry point for the application.
//

#include "framework.h"
#include "Win32Cpp.h"

#include <opencv2/core.hpp>
#include <opencv2/videoio.hpp>
#include <opencv2/highgui.hpp>
#include "opencv2/imgproc/imgproc.hpp"
#include <iostream>
#include <stdio.h>

using namespace cv;
using namespace std;
Mat rawFrame;
VideoCapture cap;
int indexPalette = 0;
TCHAR itemPalette[][64] = { L"None", L"Autumn", L"Bone", L"Jet", L"Winter", L"RainBow", L"Ocean", L"Summer", L"Spring", L"Cool", 
                            L"HSV", L"Pink", L"Hot", L"Parula", L"Magma", L"Inferno", L"Plasma", L"Viridis", L"Cividis", L"Twilight"
                            };

// Global Variables:
HWND hDlgMain;                                // current instance
INT_PTR CALLBACK MainDlgProc(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam);
void CALLBACK frameTimerProc(HWND hWnd, UINT nMsg, UINT_PTR nIDEvent, DWORD dwTime);

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPWSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    // TODO: Place code here.
    DialogBox(hInstance, MAKEINTRESOURCE(IDD_MAIN_DIALOG), HWND_DESKTOP, MainDlgProc);

    return 0;
}

// Message handler for about box.
INT_PTR CALLBACK MainDlgProc(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);

    switch (message)
    {
        case WM_INITDIALOG:
        {
            hDlgMain = hDlg;

            for (int i = 0; i < 20; i++)
            {
                SendMessage(GetDlgItem(hDlg, IDC_COMBO_PALETTE), CB_ADDSTRING, 0, (LPARAM)itemPalette[i]);
            }
            SetTimer(hDlg, 1, 110, frameTimerProc);

            for (int deviceID = 0; deviceID < 10; deviceID++)
            {
                int apiID = cv::CAP_ANY;      // 0 = autodetect default API
                // open selected camera using selected API
                cap.open(deviceID + apiID);
                // check if we succeeded
                if (!cap.isOpened()) {
                    cerr << "ERROR! Unable to open camera\n";
                }
                break;
            }

            if (cap.isOpened()) 
            {
                cap.set(cv::CAP_PROP_FOURCC, VideoWriter::fourcc('Y', '1', '6', ' '));
                cap.set(cv::CAP_PROP_CONVERT_RGB, 0);
            }
            else
            {
                MessageBox(hDlg, L"Any correct camera is not found", L"Warning", MB_OK);
            }

            return (INT_PTR)TRUE;
        }

        case WM_COMMAND:
            switch (LOWORD(wParam))
            {
                case IDOK:
                case IDCANCEL:
                    KillTimer(hDlg, 1);
                    EndDialog(hDlg, LOWORD(wParam));
                    return (INT_PTR)TRUE;    

                case IDC_COMBO_PALETTE:
                    if (HIWORD(wParam) == CBN_SELCHANGE)
                    {
                        indexPalette = SendMessage(GetDlgItem(hDlg, IDC_COMBO_PALETTE), CB_GETCURSEL, 0, 0);
                    }
                    break;
            }
            break;

        case WM_CTLCOLORSTATIC:
            SetBkMode((HDC)wParam, TRANSPARENT);

            if ((HWND)lParam == GetDlgItem(hDlg, IDC_LABEL_MAX_TEMP))
            {
                SetTextColor((HDC)wParam, RGB(255, 10, 10));
                return (BOOL)CreateSolidBrush(GetSysColor(COLOR_MENU));
            }else
            if ((HWND)lParam == GetDlgItem(hDlg, IDC_LABEL_MIN_TEMP))
            {
                SetTextColor((HDC)wParam, RGB(10, 10, 255));
                return (BOOL)CreateSolidBrush(GetSysColor(COLOR_MENU));
            }
            break;

    }
    return (INT_PTR)FALSE;
}

void CALLBACK frameTimerProc(HWND hDlg, UINT nMsg, UINT_PTR nIDEvent, DWORD dwTime)
{
    if (cap.isOpened())
    {
        cap.read(rawFrame);
        // check if we succeeded
        if (rawFrame.empty()) {
            cerr << "ERROR! blank frame grabbed\n";
            return;
        }

        double minVal, maxVal;
        cv::minMaxLoc(rawFrame, &minVal, &maxVal);

        TCHAR strBuff[1024];
        _stprintf_s(strBuff, TEXT("%.2f กษ"), maxVal / 100 - 273.15);
        SetDlgItemText(hDlg, IDC_LABEL_MAX_TEMP, strBuff);
        _stprintf_s(strBuff, TEXT("%.2f กษ"), minVal / 100 - 273.15);
        SetDlgItemText(hDlg, IDC_LABEL_MIN_TEMP, strBuff);

#if 1
        // using picture control
        cv::normalize(rawFrame, rawFrame, 0, 255, cv::NORM_MINMAX);

        Mat frame;
        Mat tempFrame;
        rawFrame.clone().convertTo(tempFrame, CV_8U);
        cv::cvtColor(tempFrame, frame, COLOR_GRAY2RGB);

        if (indexPalette > 0)
        {
            applyColorMap(frame, frame, indexPalette - 1);  // zero based
        }

        // Make bitmapinfo(data header)
        BITMAPINFO bitInfo;
        bitInfo.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
        bitInfo.bmiHeader.biBitCount = 8 * frame.channels() * (frame.depth() + 1);
        bitInfo.bmiHeader.biWidth = frame.cols;
        bitInfo.bmiHeader.biHeight = -frame.rows; //Note the "-" sign (draw when facing a positive number)
        bitInfo.bmiHeader.biPlanes = 1;
        bitInfo.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
        bitInfo.bmiHeader.biCompression = BI_RGB;
        bitInfo.bmiHeader.biClrImportant = 0;
        bitInfo.bmiHeader.biClrUsed = 0;
        bitInfo.bmiHeader.biSizeImage = 0;
        bitInfo.bmiHeader.biXPelsPerMeter = 0;
        bitInfo.bmiHeader.biYPelsPerMeter = 0;

        RECT rect;
        GetClientRect(GetDlgItem(hDlg, IDC_CAMERA_PREVIEW), &rect);
        HDC hDC = GetDC(GetDlgItem(hDlg, IDC_CAMERA_PREVIEW));

        StretchDIBits(
            hDC,
            0, 0, rect.right - rect.left, rect.bottom - rect.top,
            0, 0, frame.cols, frame.rows,
            frame.data,
            &bitInfo,
            DIB_RGB_COLORS,
            SRCCOPY
        );

        ReleaseDC(hDlg, hDC);
#else
        // using opencv window
        cv::normalize(rawFrame, rawFrame, 20000, 65535, cv::NORM_MINMAX);
        imshow("Live", rawFrame);
#endif
    }
}
