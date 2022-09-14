import 'package:ddpoliglot_v6_flutter/exeptions/custom_exception.dart';

import '../models/data/user_language_level.dart';
import '../providers/auth/auth_state.dart';
import '../services/auth_service.dart';

class AuthRepository {
  Future<AuthState> authenticate(String email, String password, bool asAnonim,
      bool isSocialNetwork, bool isAuth) async {
    var responseData = !isAuth // not logged yet
        ? asAnonim
            ? await AuthService.authenticateAnonim()
            : isSocialNetwork
                ? await AuthService.authenticateSocialNetwork(
                    email) // via google
                : await AuthService.authenticate(email, password)
        : // already logged
        asAnonim
            ? throw Exception("User is anonim already")
            : isSocialNetwork
                ? await AuthService.authenticateSocialNetworkForAnonim(
                    email) // via google. user is anonim now
                : await AuthService.authenticate(
                    email, password); // via name-pass. user is anonim now

    if (responseData['error'] != null) {
      throw CustomException(responseData['error']['message']);
    }

    var y = responseData['expiresYear'];
    var m = responseData['expiresMonth'];
    var d = responseData['expiresDay'];
    var h = responseData['expiresHour'];
    var mm = responseData['expiresMinute'];

    final expiryDate = DateTime(y, m, d, h, mm);

    var lst = responseData['roles'] == null
        ? []
        : responseData['roles'] as List<dynamic>;
    final roles = lst.map((e) => e.toString());

    final userLanguageLevels =
        (responseData['userLanguageLevels'] as List<dynamic>)
            .map((e) => UserLanguageLevel.fromJson(e as Map<String, dynamic>))
            .toList();

    return AuthState(
        token: responseData['token'],
        expiryDate: expiryDate,
        userId: responseData['userId'],
        userName: responseData['userName'],
        roles: roles.toList(),
        userLanguageLevels: userLanguageLevels,
        isNeedAuthAnonim: false);
  }

  Future<void> signup(String email, String password, bool isAuth) async {
    return !isAuth
        ? await AuthService.register(email, password)
        : await AuthService.registerForAnonim(email, password);
  }
}
