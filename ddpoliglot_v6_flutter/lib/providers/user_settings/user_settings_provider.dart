// ignore_for_file: file_names

import 'package:flutter/material.dart';

import '../../models/user_settings_state_data.dart';
import '../../repositories/store_local_repository.dart';
import '../auth/auth_state.dart';
//import 'package:shared_preferences/shared_preferences.dart';

class UserSettingsProvider with ChangeNotifier {
  UserSettingsState _state = UserSettingsState.initial();
  UserSettingsState get state => _state;

  final AuthState authState;

  UserSettingsProvider(
      {required this.authState, required UserSettingsState newState}) {
    _state = newState;
  }

  Future<bool> tryLoadFromStore() async {
    final savedStateData =
        await StoreLocalRepository.getUserSettingsState(authState);
    if (savedStateData == null) {
      return false;
    }

    _state = savedStateData;
    debugPrint('UserSettings tryLoginFromStore true');
    debugPrint('********* +++++ UserSettingsProviderV7 notifyListeners 1');
    notifyListeners();
    return true;
  }

  Future<void> update(UserSettingsState settings, {bool notify = true}) async {
    _state = settings;
    await StoreLocalRepository.saveUserSettingsState(_state, authState);
    if (notify) {
      debugPrint('Settings saved with notif');
      debugPrint('********* +++++ UserSettingsProviderV7 notifyListeners 2');
      notifyListeners();
    }
  }

  Future<void> clean() async {
    _state = UserSettingsState.initial();
    await StoreLocalRepository.saveUserSettingsState(_state, authState);
    debugPrint('Settings clean with notif');
    debugPrint('********* +++++ UserSettingsProviderV7 notifyListeners 3');
    notifyListeners();
  }
}
