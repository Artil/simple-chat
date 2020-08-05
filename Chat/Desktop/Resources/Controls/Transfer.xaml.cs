using ChatDesktop.Models;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;

namespace ChatDesktop.Resources.Controls
{
    /// <summary>
    /// Логика взаимодействия для Transfer.xaml
    /// </summary>
    public partial class Transfer : UserControl, INotifyPropertyChanged
    {
        public Transfer()
        {
            InitializeComponent();
        }

        private ICommand _addItems;
        public ICommand AddItemsCommand => _addItems == null ? _addItems = new RelayCommand(AddItemsAction) : _addItems;

        private ICommand _removeItems;
        public ICommand RemoveItemsCommand => _removeItems == null ? _removeItems = new RelayCommand(RemoveItemsAction) : _removeItems;

        public IList ListSource
        {
            get => (IList)GetValue(ListSourceProperty);
            set
            {
                base.SetValue(Transfer.ListSourceProperty, value);
                OnPropertyChanged();
            }
        }

        public IList SelectedSource
        {
            get => (IList)GetValue(SelectedSourceProperty) ?? new List<object>();
            set
            {
                base.SetValue(Transfer.SelectedSourceProperty, value);
                OnPropertyChanged();
            }
        }

        public static DependencyProperty ListSourceProperty = DependencyProperty.Register(
             "ListSource",
             typeof(IList),
             typeof(Transfer),
             new PropertyMetadata(OnListValueChanged));

        public static DependencyProperty SelectedSourceProperty = DependencyProperty.Register(
             "SelectedSource",
              typeof(IList),
              typeof(Transfer),
              new PropertyMetadata(OnSelectedValueChanged));

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate", 
            typeof(DataTemplate), 
            typeof(Transfer), 
            new UIPropertyMetadata(null));

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set 
            { 
                SetValue(ItemTemplateProperty, value);
                OnPropertyChanged();
            }
        }

        private static void OnListValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Transfer)d).ListSource = (IList)e.NewValue;
        }

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Transfer)d).SelectedSource = (IList)e.NewValue;
        }

        private void AddItemsAction(object obj)
        {
            for (int i = AvaibleList?.SelectedItems.Count-1 ?? 0; i >= 0; i--)
            {
                SelectedSource.Add(AvaibleList.SelectedItems[i]);
                ListSource.Remove(AvaibleList.SelectedItems[i]);
            }
        }

        private void RemoveItemsAction(object obj)
        {
            for (int i = ReturnList?.SelectedItems.Count - 1 ?? 0; i >= 0; i--)
            {
                ListSource.Add(ReturnList.SelectedItems[i]);
                SelectedSource.Remove(ReturnList.SelectedItems[i]);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
