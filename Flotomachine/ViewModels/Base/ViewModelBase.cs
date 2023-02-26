using ReactiveUI;

namespace Flotomachine.ViewModels;

public class ViewModelBase : ReactiveObject
{
	public ViewModelBase()
	{

	}

	public virtual void OnDestroy()
	{

	}
}