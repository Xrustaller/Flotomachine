using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Flotomachine.Services;
using Iot.Device.Rfid;
using System;
using System.ComponentModel;
using System.Threading;

namespace Flotomachine.View;

public partial class ReadCardWindow : Window
{
    private readonly Thread _thread;

    public ReadCardWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        _thread = new Thread(ReadCard);
        _thread.Start();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ReadCardEnd(byte[] id)
    {
        Close(id);
    }

    private async void ReadCard()
    {
#if DEBUG
        try
        {
            Thread.Sleep(2000);
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                ReadCardEnd(new byte[] { 255, 255, 255, 255 });
            });
        }
        catch (Exception)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                ReadCardEnd(null);
            });
        }
        return;
#endif
#pragma warning disable CS0162
        using CardIdService service = new CardIdService();
        string version = service.CreateConnection();

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            Title = $"������ v{version}";
        });

        Data106kbpsTypeA card = service.ReadCard();
        if (card == null)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                ReadCardEnd(null);
            });
        }
        else
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                ReadCardEnd(card.NfcId);
            });
        }
#pragma warning restore CS0162
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        _thread?.Interrupt();
        base.OnClosing(e);
    }

    private void Cancel_OnClick(object sender, RoutedEventArgs e)
    {
        ReadCardEnd(null);
    }
}