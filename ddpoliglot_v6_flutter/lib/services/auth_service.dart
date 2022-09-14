// ignore_for_file: file_names

import 'dart:convert';

import '../utils/http_utils.dart';

class AuthService {
  static Future<dynamic> authenticate(String email, String password) async {
    final body = json.encode({
      'UserName': email,
      'Password': password,
    });

    return HttpUtils.post('Authorization/GetToken', body);
  }

  static Future<dynamic> register(String email, String password) async {
    final body = json.encode({
      'UserName': email,
      'Password': password,
    });

    return HttpUtils.post('Authorization/Register', body);
  }

  static Future<dynamic> registerForAnonim(
      String email, String password) async {
    final body = json.encode({
      'UserName': email,
      'Password': password,
    });

    return HttpUtils.post('Authorization/RegisterForAnonim', body);
  }

  static Future<dynamic> authenticateAnonim() {
    final body = json.encode({
      'UserName': 'email',
      'Password': 'password',
    });
    return HttpUtils.post('Authorization/RegisterAnonimusAndGetToken', body);
  }

  static Future<dynamic> authenticateSocialNetwork(String email) {
    final body = json.encode({
      'UserName': email,
    });
    return HttpUtils.post('Authorization/AuthenticateSocialNetwork', body);
  }

  static Future<dynamic> authenticateSocialNetworkForAnonim(String email) {
    final body = json.encode({
      'UserName': email,
    });
    return HttpUtils.post(
        'Authorization/AuthenticateSocialNetworkForAnonim', body);
  }
}
