using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RepairTracking.Helpers;
using RepairTracking.Models;
using RepairTracking.Repositories.Abstract;
using RepairTracking.Services;
using RepairTracking.ViewModels.Factories;

namespace RepairTracking.ViewModels;

public partial class UserViewModel
    : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<UserInfo> _users;
    [ObservableProperty] private UserInfo? _selectedUser;
    private readonly IUserRepository _userRepository;
    private readonly IDialogService _dialogService;
    private readonly IViewModelFactory _viewModelFactory;

    public UserViewModel(IUserRepository userRepository, IDialogService dialogService,
        IViewModelFactory viewModelFactory)
    {
        _userRepository = userRepository;
        _dialogService = dialogService;
        _viewModelFactory = viewModelFactory;
        LoadUsers();
    }

    [RelayCommand]
    private async Task OpenAddOrUpdateUserWindow(UserInfo? userInfo)
    {
        var viewModel = _viewModelFactory.CreateAddOrUpdateUserViewModel(userInfo?.Id,
            userInfo?.Name, userInfo?.Surname, userInfo?.PhoneNumber, userInfo?.Email);
        await _dialogService.OpenAddOrUpdateUserDialogWindow(viewModel);
        LoadUsers();
    }

    [RelayCommand]
    private async Task DeleteUser(UserInfo? userInfo)
    {
        if (userInfo is not null)
        {
            var result = await _dialogService.YesNoMessageBox(
                $"Seçili kullanıcıyı silmek istediğinize emin misiniz? {userInfo.Fullname}",
                MessageTitleType.WarningTitle);

            if (result)
            {
                _userRepository.DeleteUser(userInfo.Id);
                await _dialogService.OkMessageBox("Kullanıcı başarıyla silindi.", MessageTitleType.SuccessTitle);
                LoadUsers();
            }
        }
    }

    private void LoadUsers()
    {
        var usersList = _userRepository.GetActiveUsers();
        Users = new ObservableCollection<UserInfo>(usersList);
    }
}