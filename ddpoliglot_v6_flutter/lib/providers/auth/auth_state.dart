import 'dart:convert';
import 'package:equatable/equatable.dart';

import '../../models/data/user_language_level.dart';

class AuthState extends Equatable {
  final String token;
  final DateTime expiryDate;
  final String userId;
  final String userName;
  final List<String> roles;
  final List<UserLanguageLevel> userLanguageLevels;
  final bool isNeedAuthAnonim;
  const AuthState({
    required this.token,
    required this.expiryDate,
    required this.userId,
    required this.userName,
    required this.roles,
    required this.userLanguageLevels,
    required this.isNeedAuthAnonim,
  });

  factory AuthState.initial() {
    return AuthState(
        token: '',
        expiryDate: DateTime.fromMicrosecondsSinceEpoch(0),
        userId: '',
        userName: '',
        roles: const [],
        userLanguageLevels: const [],
        isNeedAuthAnonim: false);
  }

  bool get isAuth {
    return currentToken.isNotEmpty;
  }

  String get currentToken {
    if (expiryDate.isAfter(DateTime.now()) && token.isNotEmpty) {
      return token;
    }

    return '';
  }

  AuthState copyWith({
    String? token,
    DateTime? expiryDate,
    String? userId,
    String? userName,
    List<String>? roles,
    List<UserLanguageLevel>? userLanguageLevels,
    bool? isNeedAuthAnonim,
  }) {
    return AuthState(
      token: token ?? this.token,
      expiryDate: expiryDate ?? this.expiryDate,
      userId: userId ?? this.userId,
      userName: userName ?? this.userName,
      roles: roles ?? this.roles,
      userLanguageLevels: userLanguageLevels ?? this.userLanguageLevels,
      isNeedAuthAnonim: isNeedAuthAnonim ?? this.isNeedAuthAnonim,
    );
  }

  Map<String, dynamic> toMap() {
    final result = <String, dynamic>{};

    result.addAll({'token': token});
    result.addAll({'expiryDate': expiryDate.millisecondsSinceEpoch});
    result.addAll({'userId': userId});
    result.addAll({'userName': userName});
    result.addAll({'roles': roles});
    result.addAll({
      'userLanguageLevels': userLanguageLevels.map((x) => x.toMap()).toList()
    });
    result.addAll({'isNeedAuthAnonim': isNeedAuthAnonim});

    return result;
  }

  factory AuthState.fromMap(Map<String, dynamic> map) {
    return AuthState(
      token: map['token'],
      expiryDate: DateTime.fromMillisecondsSinceEpoch(map['expiryDate']),
      userId: map['userId'],
      userName: map['userName'],
      roles: List<String>.from(map['roles']),
      userLanguageLevels: List<UserLanguageLevel>.from(
          map['userLanguageLevels']?.map((x) => UserLanguageLevel.fromMap(x))),
      isNeedAuthAnonim: map['isNeedAuthAnonim'] ?? false,
    );
  }

  String toJson() => json.encode(toMap());

  factory AuthState.fromJson(String source) =>
      AuthState.fromMap(json.decode(source));

  @override
  String toString() {
    return 'AuthState(token: $token, expiryDate: $expiryDate, userId: $userId, userName: $userName, roles: $roles, userLanguageLevels: $userLanguageLevels, isNeedAuthAnonim: $isNeedAuthAnonim)';
  }

  @override
  List<Object> get props {
    return [
      token,
      expiryDate,
      userId,
      userName,
      roles,
      userLanguageLevels,
      isNeedAuthAnonim,
    ];
  }
}
