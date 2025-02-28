﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Module08.Model;
using Module08.Services;

namespace Module08.ViewModel
{
    public class UserViewModel : BindableObject
    {
        private readonly UserService _userService;
        public ObservableCollection<User> Users { get; set; }
        private User? _selectedUser;
        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                UpdateEntryField();
            }
        }

        private string? _nameInput = string.Empty;
        public string? NameInput
        {
            get => _nameInput;
            set
            {
                _nameInput = value;
                OnPropertyChanged();
            }
        }

        private string? _genderInput = string.Empty;
        public string? GenderInput
        {
            get => _genderInput;
            set
            {
                _genderInput = value;
                OnPropertyChanged();
            }
        }

        private string? _contactNoInput = string.Empty;
        public string? ContactNoInput
        {
            get => _contactNoInput;
            set
            {
                _contactNoInput = value;
                OnPropertyChanged();
            }
        }

        private void ClearInput()
        {
            NameInput = string.Empty;
            GenderInput = string.Empty;
            ContactNoInput = string.Empty;
        }

        private void UpdateEntryField()
        {
            if (SelectedUser != null)
            {
                NameInput = SelectedUser.Name;
                GenderInput = SelectedUser.Gender;
                ContactNoInput = SelectedUser.ContactNo;
            }
            else
            {
                ClearInput();
            }
        }

        public UserViewModel()
        {
            _userService = new UserService();
            Users = new ObservableCollection<User>();
            LoadUserCommand = new Command(async () => await LoadUsers());
            AddUserCommand = new Command(async () => await AddUser());
            DeleteUserCommand = new Command(async () => await DeleteUser());
            UpdateUserCommand = new Command(async () => await UpdateUser());
        }

        public ICommand LoadUserCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand UpdateUserCommand { get; }

        public async Task UpdateUser()
        {
            if (SelectedUser != null)
            {
                SelectedUser.Name = NameInput;
                SelectedUser.Gender = GenderInput;
                SelectedUser.ContactNo = ContactNoInput;

                var result = await _userService.UpdateUserAsync(SelectedUser);
                await LoadUsers();
            }
        }

        private async Task DeleteUser()
        {
            if (SelectedUser != null)
            {
                var result = await _userService.DeleteUserAsync(SelectedUser.ID);
                await LoadUsers();
            }
        }

        private async Task LoadUsers()
        {
            var users = await _userService.GetUsersAsync();
            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }

        private async Task AddUser()
        {
            if (!string.IsNullOrWhiteSpace(NameInput) &&
                !string.IsNullOrWhiteSpace(GenderInput) &&
                !string.IsNullOrWhiteSpace(ContactNoInput))
            {
                var newUser = new User
                {
                    Name = NameInput,
                    Gender = GenderInput,
                    ContactNo = ContactNoInput
                };
                var result = await _userService.AddUserAsync(newUser);
                if (result.Equals("Success", StringComparison.OrdinalIgnoreCase))
                {
                    await LoadUsers();
                }
            }
        }
    }
}