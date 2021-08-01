# MVVMRelayCommandSample

## Command 패턴의 이해와 데이터 바인딩
- MVVM에서는 UI 버튼을 클릭 시 발생하는 Click 이벤트 핸들러를 이용하기 보다는 Command를 이용하기를 권장한다.
- 여러 버튼에서 하나의 Command를 공유할 수 있으므로 모든 컨트롤마다 Click 이벤트를 만드는 방법보다 효율적이기 때문이다.

## XAML
- TextBox는 ViewModel의 SelectedEmp Property의 Name을 바인딩한다.   
- Button은 ViewModel에 있는 AddEmpCommand에 바인딩되어있다.
- Button의 Parameter는 바로 상단에 있는 TextBox{x:txtName}를 바인딩한다.
- ListBox의 ItemSource는 ViewModel의 ObservableCollection의   
  PropertyName이 Emps이기 때문에 그 이름을 바인딩한 것이다.
```html
<Window x:Class="MVVMSample.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:MVVMSample"
        mc:Ignorable="d"
        Title="MainWindow" Height="319.286" Width="636.071">
    <Window.DataContext>
        <local:UserViewModel/> <!--ViewModel 삽입-->
    </Window.DataContext>
    <StackPanel>
        <TextBlock>사원 이름을 입력하세요</TextBlock>
        <TextBox x:Name="txtName" Text="{Binding SelectedEmp.Name}"/>
        <Button Command="{Binding AddEmpCommand}"
              CommandParameter="{Binding Text, ElementName=txtName}">Add</Button>
        <ListBox ItemsSource="{Binding Emps}"
                 SelectedItem="{Binding SelectedEmp}"
                 DisplayMemberPath="Name"
                 x:Name="empListBox"/>
        <Label x:Name="label" Content="{Binding SelectedItem, ElementName=empListBox}"
               HorizontalAlignment="Center"/>
    </StackPanel>
</Window>
```

## RelayCommand
- ICommand는 Execute 및 CanExecute라는 두 가지 메서드와 CanExecuteChanged 이벤트를 제공한다.   
- Execute 메서드는 실제 처리해야 하는 작업을 기술하고   
  CanExecute 메서드에서는 Execute 메서드의 코드를 실행할지 여부를 결정하는 코드를 기술한다.   
- CanExecute가 false를 리턴하면 Execute 메서드는 호출되지 않는다.   
  CanExecute 메서드는 명령을 사용하게 하거나 불가능하게 할 때 사용되며 그 여부를 확인하기위해 WPF에 의해 호출된다.   
  CanExecute는 GETPOCUS, LOSTPOCUS, Mouse-Up 등과 같은 UI 상호작용 중 대부분 발생한다.   
- CanExecute의 상태가 변경되면 CanExecuteChanged 이벤트가 발생되고   
  WPF는 구현체에서 CanExecute를 호출하고 Command에 연결된 컨트롤의 상태를 변경한다.

## CommandManager.RequerySuggested?
- CommandManager.RequerySuggested 이벤트는 CanExecute 메서드를 강제로 실행할 수 있다.
- 사용자 정의 명령의 경우 CanExecute 메서드가 대부분의 시나리오에서 호출되지는 않으므로   
  어떤 조건에 따라 버튼을 활성화, 비활성화해야할 수도 있는데   
  ICommand 구현체에서 CanExecuteChanged 이벤트를 CommandManager의 RequerySuggested 이벤트에 연결하면 된다.
- CanExecuteChanged 이벤트는 CommandManager의 RequerySuggested에 위임되어   
  모든 종류의 UI 상호작용을 통해 변경사항이 호출되는 정확한 알림을 제공한다.
- RequerySuggested 이벤트의 CommandManager.InvalidateRequerySuggested()를 호출하여   
  CommandManager의 REquerySuggested 이벤트를 발생하도록 할 수도 있다.

### 사용자 정의 명령의 경우, 
```c#
using System;
using System.Windows.Input;

namespace MVVMSample
{
    public class RelayCommand : ICommand
    {
        private Func<object, bool> canExecute;
        private Action<object> executeAction;

        public RelayCommand(Action<object> executeAction) : this(executeAction, null) // 한 개의 args를 받으면 두 번째 args는 null로 주기 위하여
        {

        }

        public RelayCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            this.executeAction = executeAction ??
                throw new ArgumentNullException("Execute Action was null for ICommanding");
            this.canExecute = canExecute; // 현재의 example에서는 null이기에
        }
        
        public bool CanExecute(object parameter)
        {
            if (parameter?.ToString().Length is 0) // parameter가 zero이면.
            {
                return false;
            }

            bool result = this.canExecute is null ? true : this.canExecute.Invoke(parameter);
            return result;
        }

        public void Execute(object parameter)
        {
            this.executeAction.Invoke(parameter);
        }
        
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }


    }
}
```
