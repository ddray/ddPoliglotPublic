import 'dart:convert';
import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter/foundation.dart' show kIsWeb;
import 'package:shared_preferences/shared_preferences.dart';
import 'package:safe_device/safe_device.dart';
import 'package:http/http.dart' as http;

import '../exeptions/custom_exception.dart';
import '../models/user_settings_state_data.dart';
import '../providers/auth/auth_state.dart';
import '../repositories/store_local_repository.dart';
import './http_error_handler.dart';

class HttpUtils {
  static bool? isRealDevice;

  static Future<String> getServerDomen() async {
    // ignore: prefer_conditional_assignment
    if (isRealDevice == null) {
      isRealDevice = await SafeDevice.isRealDevice;
    }

    return (isRealDevice ?? false) ? releaseDomen : debugDomen;
  }

  static Future<bool> getIsRealDevice() async {
    // ignore: prefer_conditional_assignment
    if (isRealDevice == null) {
      isRealDevice = await SafeDevice.isRealDevice;
    }

    return (isRealDevice ?? false);
  }

  static String debugDomen =
      kIsWeb ? 'https://localhost:44342' : 'https://10.0.2.2:44342';
  static String releaseDomen = 'https://www.ddPoliglot.com';
  bool isProd = const bool.fromEnvironment('dart.vm.product');

  static Future<String> getTokenFromPrefs() async {
    final prefs = await SharedPreferences.getInstance();
    if (!prefs.containsKey('userData')) {
      return '';
    }

    final extractedUserData =
        json.decode(prefs.getString('userData') ?? '') as Map<String, dynamic>;
    final expiryDate =
        DateTime.parse(extractedUserData['expiryDate'] as String);

    if (expiryDate.isBefore(DateTime.now())) {
      return '';
    }

    return extractedUserData['token'] as String;
  }

  static dynamic get(String urlParams) async {
    final serverDomen = await getServerDomen();
    final url = Uri.parse('$serverDomen/api/$urlParams');
    final authState = await StoreLocalRepository.getAuthState();
    var appSettings = await StoreLocalRepository.getUserSettingsState(
        authState ?? AuthState.initial());
    var appSettingsHeaderValue = appSettings == null
        ? ''
        : '${appSettings.nativeLanguage!.languageID};${appSettings.learnLanguage!.languageID}';
    final headers = !(authState?.isAuth ?? false)
        ? {
            'Content-Type': 'application/json',
          }
        : {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ${authState!.token}',
            'spa-app-settings': appSettingsHeaderValue
          };

    debugPrint('start request: ${url.toString()}');
    try {
      final response = await http.get(url, headers: headers);

      if (response.statusCode != 200) {
        if (response.statusCode == 400) {
          throw CustomException(response.body);
        } else if (response.statusCode == 401) {
          throw CustomException('Unauthorized');
        } else {
          throw HttpException(httpErrorHandler(response));
        }
      }

      debugPrint('end request: ${url.toString()}');
      return response.body.isNotEmpty ? json.decode(response.body) : null;
    } on HttpException catch (e) {
      throw HttpException(
          'request $url, header: $headers, error: ${e.toString()}');
    } catch (e) {
      rethrow;
    }
  }

  static dynamic post(String urlParams, String body) async {
    final serverDomen = await getServerDomen();
    final url = Uri.parse('$serverDomen/api/$urlParams');
    final authState = await StoreLocalRepository.getAuthState();
    var appSettings = await StoreLocalRepository.getUserSettingsState(
        authState ?? AuthState.initial());
    var appSettingsHeaderValue = appSettings == null
        ? ''
        : '${appSettings.nativeLanguage!.languageID};${appSettings.learnLanguage!.languageID}';
    final headers = !(authState?.isAuth ?? false)
        ? {
            'Content-Type': 'application/json',
          }
        : {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ${authState!.token}',
            'spa-app-settings': appSettingsHeaderValue,
          };

    debugPrint('start request: ${url.toString()}');

    try {
      final response = await http.post(url, body: body, headers: headers);

      if (response.statusCode != 200) {
        if (response.statusCode == 400) {
          throw CustomException(response.body);
        } else if (response.statusCode == 401) {
          throw CustomException('Unauthorized');
        } else {
          throw HttpException(httpErrorHandler(response));
        }
      }

      debugPrint('end request: ${url.toString()}');

      return response.body.isNotEmpty ? json.decode(response.body) : null;
    } on HttpException catch (e) {
      throw HttpException(
          'request $url, header: $headers, error: ${e.toString()}');
    } catch (e) {
      rethrow;
    }
  }

  static Future<String> getUserIdFromPrefs() async {
    final prefs = await SharedPreferences.getInstance();
    if (!prefs.containsKey('userData')) {
      return '';
    }

    final extractedUserData =
        json.decode(prefs.getString('userData') ?? '') as Map<String, dynamic>;
    final expiryDate =
        DateTime.parse(extractedUserData['expiryDate'] as String);

    if (expiryDate.isBefore(DateTime.now())) {
      return '';
    }

    return extractedUserData['userId'] as String;
  }

  static Future<UserSettingsState?> getUserSettingsStateFromPrefs() async {
    final userId = await getUserIdFromPrefs();

    String keyPref = 'userSettings$userId';
    final prefs = await SharedPreferences.getInstance();
    if (!prefs.containsKey(keyPref)) {
      return null;
    }

    final jsonValue = json.decode(prefs.getString(keyPref) ?? '');
    return UserSettingsState.fromJson(jsonValue);
  }

  static Future<bool> isOnLine() async {
    try {
      final result = await InternetAddress.lookup('example.com');
      if (result.isNotEmpty && result[0].rawAddress.isNotEmpty) {
        return true;
      }
    } on SocketException catch (_) {
      debugPrint('---------------- not connected');
    }

    return false;
  }

  static Future<bool> isOnLineDomen() async {
    try {
      final serverDomen = await getServerDomen();
      final result = await InternetAddress.lookup(serverDomen);
      if (result.isNotEmpty && result[0].rawAddress.isNotEmpty) {
        return true;
      }
    } on SocketException catch (ex) {
      debugPrint('domen ---------------- not connected ${ex.toString()}');
    }

    return false;
  }
}



  // static dynamic get(String urlParams) async {
  //   final serverDomen = await getServerDomen();
  //   final url = Uri.parse('$serverDomen/api/$urlParams');
  //   final authState = await StoreLocalRepository.getAuthState();
  //   var _appSettings = await StoreLocalRepository.getUserSettingsState(
  //       authState ?? authState.initial());
  //   var _appSettingsHeaderValue = _appSettings == null
  //       ? ''
  //       : '${_appSettings.nativeLanguage!.languageID};${_appSettings.learnLanguage!.languageID}';
  //   final headers = !(authState?.isAuth ?? false)
  //       ? {
  //           'Content-Type': 'application/json',
  //         }
  //       : {
  //           'Content-Type': 'application/json',
  //           'Authorization': 'Bearer ${authState!.token}',
  //           'spa-app-settings': _appSettingsHeaderValue
  //         };

  //   debugPrint('start request: ${url.toString()}');
  //   final response = await http.get(url, headers: headers);

  //   if (response.statusCode != 200) {
  //     throw Exception(httpErrorHandler(response));
  //   }

  //   debugPrint('end request: ${url.toString()}');
  //   return response.body.isNotEmpty ? json.decode(response.body) : null;
  // }

  // static dynamic post(String urlParams, String body) async {
  //   final serverDomen = await getServerDomen();
  //   final url = Uri.parse('$serverDomen/api/$urlParams');
  //   final authState = await StoreLocalRepository.getAuthState();
  //   var _appSettings = await StoreLocalRepository.getUserSettingsState(
  //       authState ?? authState.initial());
  //   var _appSettingsHeaderValue = _appSettings == null
  //       ? ''
  //       : '${_appSettings.nativeLanguage!.languageID};${_appSettings.learnLanguage!.languageID}';
  //   final headers = !(authState?.isAuth ?? false)
  //       ? {
  //           'Content-Type': 'application/json',
  //         }
  //       : {
  //           'Content-Type': 'application/json',
  //           'Authorization': 'Bearer ${authState!.token}',
  //           'spa-app-settings': _appSettingsHeaderValue,
  //         };

  //   debugPrint('start request: ${url.toString()}');

  //   final response = await http.post(url, body: body, headers: headers);

  //   if (response.statusCode != 200) {
  //     throw Exception(httpErrorHandler(response));
  //   }

  //   debugPrint('end request: ${url.toString()}');

  //   return response.body.isNotEmpty ? json.decode(response.body) : null;
  // }
