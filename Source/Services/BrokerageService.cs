using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace FastBuild.Dashboard.Services
{
	internal class BrokerageService : IBrokerageService
	{
		private const string WorkerPoolSearchPattern = "*.windows";

		private string[] _workerNames;

		public string[] WorkerNames
		{
			get => _workerNames;
			private set
			{
				var oldCount = _workerNames.Length;
				_workerNames = value;

				if (oldCount != _workerNames.Length)
				{
					this.WorkerCountChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		private bool _isUpdatingWorkers;

		public string BrokeragePath
		{
			get => Environment.GetEnvironmentVariable("FASTBUILD_BROKERAGE_PATH");
			set => Environment.SetEnvironmentVariable("FASTBUILD_BROKERAGE_PATH", value);
		}

		public event EventHandler WorkerCountChanged;

		public BrokerageService()
		{
			_workerNames = new string[0];

			var checkTimer = new Timer(5000);
			checkTimer.Elapsed += this.CheckTimer_Elapsed;
			checkTimer.AutoReset = true;
			checkTimer.Enabled = true;
			this.UpdateWorkers();
		}

		private void CheckTimer_Elapsed(object sender, ElapsedEventArgs e) => this.UpdateWorkers();

		private void UpdateWorkers()
		{
			if (_isUpdatingWorkers)
				return;

			_isUpdatingWorkers = true;

			try
			{
				var brokeragePath = this.BrokeragePath;
				if (string.IsNullOrEmpty(brokeragePath))
				{
					this.WorkerNames = new string[0];
					return;
				}

				try
				{
					var directories = Directory.GetDirectories(Path.Combine(brokeragePath, "main"), WorkerPoolSearchPattern);

					if (directories.Length == 0)
					{
						return;
					}

					Array.Sort(directories);
					string workerPoolDirectory = directories[directories.Length - 1];

					var tempList = new List<string>();
					var workers = Directory.GetFiles(workerPoolDirectory);

					foreach (var workerFile in workers)
					{
						var lines = System.IO.File.ReadAllLines(workerFile);
						foreach (var line in lines)
						{
							if (line.StartsWith("User"))
							{
								var filename = Path.GetFileName(workerFile);
								var user = line.Replace("User:", string.Empty).Trim();
								var name = $"{user} ({filename})";
								tempList.Add(name);
								break;
							}
						}
					}

					this.WorkerNames = tempList.ToArray();
				}
				catch (IOException)
				{
					this.WorkerNames = new string[0];
				}
			}
			finally
			{
				_isUpdatingWorkers = false;
			}
		}
	}
}
