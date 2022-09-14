import 'dart:convert';

import '../models/user_settings_state_data.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../providers/auth/auth_state.dart';

class StoreLocalRepository {
  static const String kNameAuthStateStore = 'AuthState';
  static const String kNameUserSettingsDataStore = 'UserSettingsData';

  // Auth
  static Future<void> saveAuthState(AuthState data) async {
    final prefs = await SharedPreferences.getInstance();
    prefs.setString(kNameAuthStateStore, data.toJson());
  }

  static Future<AuthState?> getAuthState() async {
    final prefs = await SharedPreferences.getInstance();
    if (!prefs.containsKey(kNameAuthStateStore)) {
      return null;
    }

    return AuthState.fromJson(prefs.getString(kNameAuthStateStore)!);
  }

  static Future<void> clearAuthState() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(kNameAuthStateStore);
  }

  // UserSettings
  static Future<void> saveUserSettingsState(
      UserSettingsState data, AuthState authState) async {
    final storeName = '${kNameUserSettingsDataStore}_${authState.userId}';
    final prefs = await SharedPreferences.getInstance();
    prefs.setString(storeName, json.encode(data));
  }

  static Future<UserSettingsState?> getUserSettingsState(
      AuthState authState) async {
    final storeName = '${kNameUserSettingsDataStore}_${authState.userId}';
    final prefs = await SharedPreferences.getInstance();
    if (!prefs.containsKey(storeName)) {
      return null;
    }

    return UserSettingsState.fromJson(json.decode(prefs.getString(storeName)!));
  }

  static Future<void> clearUserSettingsState(AuthState authState) async {
    final storeName = '${kNameUserSettingsDataStore}_${authState.userId}';
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(storeName);
  }
}
