import 'dart:async';
import 'package:ddpoliglot_v6_flutter/providers/auth/auth_state.dart';
import 'package:flutter/widgets.dart';
import 'package:google_sign_in/google_sign_in.dart';

import '../../repositories/auth_repository.dart';
import '../../repositories/store_local_repository.dart';

class AuthProvider with ChangeNotifier {
  AuthState _state = AuthState.initial();
  AuthState get state => _state;

  final AuthRepository authRepository;
  AuthProvider({
    required this.authRepository,
  });

  Timer? _authTimer;

  setIsNeedAuthAnonim(bool val) {
    _state = _state.copyWith(
      isNeedAuthAnonim: val,
    );
    debugPrint('********* +++++ AuthProvider notifyListeners 1');
    notifyListeners();
  }

  authAnonim() {
    // go to login page
    setIsNeedAuthAnonim(true);
  }

  Future<void> _authenticate(String email, String password,
      {bool asAnonim = true, bool isSocialNetwork = false}) async {
    final data = await authRepository.authenticate(
        email, password, asAnonim, isSocialNetwork, state.isAuth);

    _state = data;

    _runAutoLogoutTimer();

    await StoreLocalRepository.saveAuthState(state);
    debugPrint('********* +++++ AuthProvider notifyListeners 2');
    notifyListeners();
  }

  Future<void> signup(String email, String password) async {
    await authRepository.signup(email, password, state.isAuth);
  }

  Future<void> login(String email, String password) async {
    return _authenticate(email, password, asAnonim: false);
  }

  Future<void> loginAnonim() async {
    return _authenticate('', '', asAnonim: true);
  }

  Future<void> loginGoogle(GoogleSignInAccount? account) async {
    return _authenticate(account!.email, 'GoogleSignInAccount',
        asAnonim: false, isSocialNetwork: true);
  }

  Future<void> loginFacebook(String email) async {
    return _authenticate(email, 'FacebookSignInAccount',
        asAnonim: false, isSocialNetwork: true);
  }

  Future<bool> tryLoadFromStore() async {
    final savedStateData = await StoreLocalRepository.getAuthState();
    if (savedStateData == null) {
      return false;
    }

    _state = savedStateData;

    _runAutoLogoutTimer();

    debugPrint('auth tryLoadFromStore true');

    debugPrint('********* +++++ AuthProvider notifyListeners 3');
    notifyListeners();
    return true;
  }

  Future<void> logout() async {
    _state = AuthState.initial();
    if (_authTimer != null) {
      _authTimer!.cancel();
      _authTimer = null;
    }

    await StoreLocalRepository.clearAuthState();
    debugPrint('!!!!! logout');

    debugPrint('********* +++++ AuthProvider notifyListeners 4');
    notifyListeners();
  }

  void _runAutoLogoutTimer() {
    if (_authTimer != null) {
      _authTimer!.cancel();
    }
    debugPrint('!!!!! _runAutoLogoutTimer()');
    final timeToExpiry = _state.expiryDate.difference(DateTime.now()).inSeconds;
    _authTimer = Timer(Duration(seconds: timeToExpiry), () async {
      debugPrint('!!!!! runAutoLogoutTimer() run logout');
      await logout();
    });
  }
}
