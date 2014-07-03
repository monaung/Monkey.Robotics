﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Robotics.Core.Bluetooth.LE;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Xamarin.Robotics.BluetoothLEExplorerForms
{	
	public partial class ServiceList : ContentPage
	{	
		IAdapter adapter;
		IDevice device;

		List<IService> services;

		public ServiceList (IAdapter adapter, IDevice device)
		{
			InitializeComponent ();
			this.adapter = adapter;
			this.device = device;
			this.services = new List<IService> ();

			adapter.DeviceConnected += (sender, e) => {
				//device = e.Device; // do we need to overwrite this?
				services = (List<IService>)device.Services;
				Device.BeginInvokeOnMainThread(() => {
					if (services.Count == 0) {
						DisplayAlert ("No Services Found", "No services are available for " + e.Device.Name, "OK", null);
					} else {
						listView.ItemsSource = services;
					}
				});
			};
			// TODO: add to IAdapter first
			//adapter.DeviceFailedToConnect += (sender, else) => {};

			DisconnectButton.Activated += (sender, e) => {
				adapter.DisconnectDevice (device);
			};
		}


		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			if (services.Count == 0) {
				Debug.WriteLine ("No services, attempting to connect to device");
				adapter.ConnectToDevice (device); 
			}
		}
		public void OnItemSelected (object sender, SelectedItemChangedEventArgs e) {
			((ListView)sender).SelectedItem = null; // clear selection

			var service = e.SelectedItem as IService;
			var characteristicsPage = new CharacteristicList(adapter, device, service);
			Navigation.PushAsync(characteristicsPage);
		}
	}
}

