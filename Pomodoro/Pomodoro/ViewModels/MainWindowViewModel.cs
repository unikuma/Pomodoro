using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;
using Pomodoro.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Pomodoro.ViewModels
{
	public class MainWindowViewModel : ViewModel, IDisposable
	{
		// Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).
		public void Initialize()
		{
			BackgroundColor = TomatoRed;

			MainTimer.Interval = 1000;
			MessageTimer.Interval = 300000;

			MainTimer.Elapsed += MainTimer_Elapsed;
			MessageTimer.Elapsed += MessageTimer_Elapsed;

			MainTimer.Start();
			MessageTimer.Start();

			MainTimer_Elapsed(null, null);
			MessageTimer_Elapsed(null, null);
		}

		private void MainTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			Parallel.Invoke(UpdateTimeLeft, ChangeStemAngle, CheckTimeLeft);

			void UpdateTimeLeft()
			{
				if (PomodoroSwitch)
					TimeLeft = TimeSpan.FromSeconds(--Pomodoro).ToString("mm\\：ss");
				else
					TimeLeft = TimeSpan.FromSeconds(--Break).ToString("mm\\：ss");
			}

			void ChangeStemAngle()
			{
				StemAngle = StemAngle >= 354 ? 0 : StemAngle + 6;
			}

			void CheckTimeLeft()
			{
				if (Pomodoro == 0)
				{
					PomodoroSwitch = false;
					Pomodoro = 1500;
					if (System.IO.File.Exists("BreakOn.wav")) new SoundPlayer("BreakOn.wav").Play();
				}
				else if (Break == 0)
				{
					PomodoroSwitch = true;
					Break = 300;
					TomatoCount++;
					if (System.IO.File.Exists("PomodoroOn.wav")) new SoundPlayer("PomodoroOn.wav").Play();
				}
			}
		}

		private void MessageTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (IsHiddenControl != Visibility.Hidden)
			{
				string[] Message = { "ちなみに作者はペペロンチーノがすき", "今は集中、それがだいじ", "休憩もたまには必要",
									 $"トマト{TomatoCount}個＝{TimeSpan.FromMinutes(30 * TomatoCount).ToString("mm")}分",
									 "ポモドーロも美味しいよね"};
				PomodoroMes = Message[new Random().Next(Message.Length)];
			}
		}

		private System.Timers.Timer MainTimer = new System.Timers.Timer();
		private System.Timers.Timer MessageTimer = new System.Timers.Timer();

		private int Pomodoro = 1500;
		private int Break = 300;

		private int _TomatoCount = 0;
		public int TomatoCount
		{
			get => _TomatoCount;
			set => RaisePropertyChangedIfSet(ref _TomatoCount, value);
		}

		private bool _PomodoroSwitch = true;
		public bool PomodoroSwitch
		{
			get => _PomodoroSwitch;
			set
			{
				BackgroundColor = value ? TomatoRed : TomatoGreen;
				RaisePropertyChangedIfSet(ref _PomodoroSwitch, value);
			}
		}

		private string _TimeLeft;
		public string TimeLeft
		{
			get => _TimeLeft;
			set => RaisePropertyChangedIfSet(ref _TimeLeft, value);
		}


		private string _PomodoroMes;
		public string PomodoroMes
		{
			get => _PomodoroMes;
			set => RaisePropertyChangedIfSet(ref _PomodoroMes, value);
		}


		private Brush TomatoRed = new SolidColorBrush(Color.FromRgb(227, 19, 0));
		private Brush TomatoGreen = new SolidColorBrush(Color.FromRgb(57, 185, 75));

		private Brush _BackgroundColor = Brushes.Black;
		public Brush BackgroundColor
		{
			get => _BackgroundColor;
			set => RaisePropertyChangedIfSet(ref _BackgroundColor, value);
		}


		private bool _TopMost = false;
		public bool TopMost
		{
			get => _TopMost;
			set => RaisePropertyChangedIfSet(ref _TopMost, value);
		}

		private Visibility _IsHiddenControl = Visibility.Visible;
		public Visibility IsHiddenControl
		{
			get => _IsHiddenControl;
			set => RaisePropertyChangedIfSet(ref _IsHiddenControl, value);
		}

		private double _StemAngle = 0;
		public double StemAngle
		{
			get => _StemAngle;
			set => RaisePropertyChangedIfSet(ref _StemAngle, value);
		}

		public void ChangeTopMost()
		{
			TopMost = !TopMost;
			SystemSounds.Beep.Play();
		}

		public void HiddenOtherControls() => IsHiddenControl = IsHiddenControl == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
	}
}
