﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using Caliburn.Micro.Validation;
using FastBuilder.Services;
using FastBuilder.Services.Worker;

namespace FastBuilder.ViewModels.Settings
{
	internal sealed class SettingsViewModel : ValidatingScreen<SettingsViewModel>, IMainPage
	{
		[CustomValidation(typeof(SettingsValidator), "ValidateBrokeragePath")]
		public string BrokeragePath
		{
			get => IoC.Get<IBrokerageService>().BrokeragePath;
			set
			{
				IoC.Get<IBrokerageService>().BrokeragePath = value;
				this.NotifyOfPropertyChange();
			}
		}

		public string DisplayWorkersInPool
		{
			get
			{
				var workerCount = IoC.Get<IBrokerageService>().WorkerNames.Length;
				switch (workerCount)
				{
					case 0:
						return "no workers in pool";
					case 1:
						return "1 worker in pool";
					default:
						return $"{workerCount} workers in pool";
				}
			}
		}

		public int WorkerMode
		{
			get => (int)IoC.Get<IWorkerAgentService>().WorkerMode;
			set
			{
				IoC.Get<IWorkerAgentService>().WorkerMode = (WorkerMode)value;
				this.NotifyOfPropertyChange();
			}
		}

		public int WorkerCores
		{
			get => IoC.Get<IWorkerAgentService>().WorkerCores;
			set
			{
				IoC.Get<IWorkerAgentService>().WorkerCores = Math.Max(1, Math.Min(this.MaximumCores, value));
				this.NotifyOfPropertyChange();
				this.NotifyOfPropertyChange(nameof(this.DisplayCores));
			}
		}

		public string DisplayCores => this.WorkerCores == 1 ? "1 core" : $"up to {this.WorkerCores} cores";

		public int MaximumCores { get; }
		public DoubleCollection CoreTicks { get; }

		public SettingsViewModel()
		{
			this.MaximumCores = Environment.ProcessorCount;
			this.CoreTicks = new DoubleCollection(Enumerable.Range(1, this.MaximumCores).Select(i => (double)i));

			this.DisplayName = "Settings";

			var brokerageService = IoC.Get<IBrokerageService>();
			brokerageService.WorkerCountChanged += this.BrokerageService_WorkerCountChanged;
		}

		private void BrokerageService_WorkerCountChanged(object sender, EventArgs e)
		{
			this.NotifyOfPropertyChange(nameof(this.DisplayWorkersInPool));
		}
	}
}
