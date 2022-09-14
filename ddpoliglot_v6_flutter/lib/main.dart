import 'dart:async';
import 'dart:io';
import 'dart:ui';

import 'package:ddpoliglot_v6_flutter/providers/playlist_provider.dart/play_list_state.dart';
import 'package:wakelock/wakelock.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'package:ddpoliglot_v6_flutter/models/data/Log.dart';
import 'package:ddpoliglot_v6_flutter/providers/auth/auth_provider.dart';
import 'package:ddpoliglot_v6_flutter/providers/database/database_provider.dart';
import 'package:ddpoliglot_v6_flutter/repositories/auth_repository.dart';
import 'package:ddpoliglot_v6_flutter/screens/user_lessons_history_screen.dart';
import 'package:ddpoliglot_v6_flutter/screens/user_lessons_results_screen.dart';
import 'package:ddpoliglot_v6_flutter/screens/user_settings_screen.dart';
import 'package:ddpoliglot_v6_flutter/services/log_service.dart';
import 'package:ddpoliglot_v6_flutter/utils/colors_utils.dart';

import './screens/home_screen.dart';
import 'models/user_settings_state_data.dart';
import 'providers/auth/auth_state.dart';
import 'providers/database/database_state.dart';
import 'providers/playlist_provider.dart/play_list_provider.dart';
import 'providers/user_settings/user_settings_provider.dart';
import 'repositories/store_local_repository.dart';

class MyHttpOverrides extends HttpOverrides {
  @override
  HttpClient createHttpClient(SecurityContext? context) {
    return super.createHttpClient(context)
      ..badCertificateCallback =
          (X509Certificate cert, String host, int port) => true;
  }
}

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  HttpOverrides.global = MyHttpOverrides();

  FlutterError.onError = (FlutterErrorDetails flutterErrorDetails) {
    FlutterError.presentError(flutterErrorDetails);
    try {
      LogService.add(Log(
          0,
          '15e4a694-819a-41ad-bc05-69e0abd36870',
          'mob err',
          'from mob (FlutterError.onError)',
          flutterErrorDetails.exceptionAsString(),
          0,
          DateTime.now()));
    } catch (_) {}
    debugPrint('$flutterErrorDetails');
  };

  PlatformDispatcher.instance.onError = (error, stack) {
    try {
      LogService.add(Log(
          0,
          '15e4a694-819a-41ad-bc05-69e0abd36870',
          'mob err',
          'from mob (onError)',
          '$error Stack Trace: $stack',
          0,
          DateTime.now()));
    } catch (_) {}
    debugPrint('$error');
    return true;
  };
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({Key? key}) : super(key: key);
  @override
  Widget build(BuildContext context) {
    Wakelock.enable();

    return MultiProvider(
      providers: [
        Provider<AuthRepository>(
          create: (context) => AuthRepository(),
        ),
        ChangeNotifierProvider<AuthProvider>(
          create: (context) {
            debugPrint('+++ AuthProvider create');
            return AuthProvider(authRepository: context.read<AuthRepository>());
          },
        ),
        ChangeNotifierProxyProvider<AuthProvider, UserSettingsProvider>(
            create: (ctx) {
          debugPrint('+++ UserSettingsProviderV7 create');
          return UserSettingsProvider(
              authState: AuthState.initial(),
              newState: UserSettingsState.initial());
        }, update: (ctx, auth, previous) {
          final isAuth = auth.state.isAuth;
          final isUserChanged =
              !(auth.state.userId == (previous?.authState.userId ?? '') &&
                  auth.state.userId.isNotEmpty);

          debugPrint(
              '+++ UserSettingsProvider update isAuth: $isAuth, isUserChanged: $isUserChanged');
          return UserSettingsProvider(
              authState: auth.state,
              newState: (isAuth && !isUserChanged && previous != null)
                  ? previous.state
                  : UserSettingsState.initial());
        }),
        ChangeNotifierProxyProvider<UserSettingsProvider, DatabaseProvider>(
          create: (ctx) {
            debugPrint('+++ DatabaseProvider create');
            return DatabaseProvider(
                authState: AuthState.initial(),
                userSettingsStateData: null,
                newState: DatabaseState.initial());
          },
          update: (ctx, userSettings, previousProvider) {
            final isAuth = userSettings.authState.isAuth;

            final isUserChanged = !(userSettings.authState.userId ==
                    (previousProvider?.authState.userId ?? '') &&
                userSettings.authState.userId.isEmpty);
            final isLngChanged =
                !((userSettings.state.nativeLanguage?.languageID ?? -1) ==
                        (previousProvider?.userSettingsStateData
                                ?.nativeLanguage!.languageID ??
                            0) &&
                    (userSettings.state.learnLanguage?.languageID ?? -1) ==
                        (previousProvider?.userSettingsStateData?.learnLanguage!
                                .languageID ??
                            0));
            final isNoChanges = (isAuth &&
                !userSettings.state.isEmpty &&
                !isUserChanged &&
                !isLngChanged);
            debugPrint(
                '+++ DatabaseProvider update isAuth: $isAuth, isUserChanged: $isUserChanged, isLngChanged: $isLngChanged');
            var newState = previousProvider == null || !isNoChanges
                ? DatabaseState.initial()
                : previousProvider.state;
            return DatabaseProvider(
                authState: userSettings.authState,
                userSettingsStateData: (isAuth && !userSettings.state.isEmpty)
                    ? userSettings.state
                    : null,
                newState: newState);
          },
        ),
        ChangeNotifierProxyProvider<DatabaseProvider, PlayListProvider>(
          create: (ctx) {
            debugPrint('+++ PlayListProvider create');
            return PlayListProvider(
                authState: AuthState.initial(),
                userSettingsStateData: null,
                databaseState: null,
                newState: PlayListState.initial());
          },
          update: (ctx, databaseProvider, previousProvider) {
            final isAuth = databaseProvider.authState.isAuth;
            final isUserChanged = !(databaseProvider.authState.userId ==
                    (previousProvider?.authState.userId ?? '') &&
                databaseProvider.authState.userId.isNotEmpty);
            final isLngChanged = !((databaseProvider.userSettingsStateData
                            ?.nativeLanguage!.languageID ??
                        -1) ==
                    (previousProvider?.userSettingsStateData?.nativeLanguage!
                            .languageID ??
                        0) &&
                (databaseProvider
                            .userSettingsStateData?.learnLanguage!.languageID ??
                        -1) ==
                    (previousProvider?.userSettingsStateData?.learnLanguage!
                            .languageID ??
                        0));

            debugPrint(
                '+++ PlayListProvider update isAuth: $isAuth, isUserChanged: $isUserChanged, isLngChanged: $isLngChanged');
            // debugPrint(
            //     '+++ PlayListProvider update previousProvider?.preparationStep ${previousProvider?.preparationStep}');
            return PlayListProvider(
                authState: databaseProvider.authState,
                userSettingsStateData:
                    isAuth ? databaseProvider.userSettingsStateData : null,
                databaseState: databaseProvider.state,
                newState: PlayListState(
                    userLessonData: (isAuth && !isUserChanged && !isLngChanged)
                        ? previousProvider?.state.userLessonData
                        : null,
                    preparationStep:
                        previousProvider?.state.preparationStep ?? 0));
          },
        ),
      ],
      child: MaterialApp(
        builder: (BuildContext context, Widget? widget) {
          ErrorWidget.builder = (errorDetails) {
            Widget error = Text(
                '...rendering error... ${errorDetails.exceptionAsString()}');
            if (widget is Scaffold || widget is Navigator) {
              error = Scaffold(body: Center(child: error));
            }

            return error;
          };

          if (widget != null) return widget;
          throw ('widget is null');
        },
        title: 'ddPoliglot V01 test',
        theme: ThemeData(
            fontFamily: 'Lato',
            primarySwatch: ColorsUtils.customYellow,
            // buttons
            floatingActionButtonTheme: const FloatingActionButtonThemeData(
              backgroundColor: Color.fromRGBO(255, 188, 59, 1),
              foregroundColor: Color.fromRGBO(72, 72, 95, 1),
            ),
            textButtonTheme: TextButtonThemeData(
                style: TextButton.styleFrom(
              textStyle: const TextStyle(
                  color: Colors.deepOrange,
                  fontSize: 20,
                  fontWeight: FontWeight.bold),
            )),
            elevatedButtonTheme: ElevatedButtonThemeData(
                style: ElevatedButton.styleFrom(
                    padding:
                        const EdgeInsets.symmetric(horizontal: 30, vertical: 8),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(10),
                    ),
                    textStyle: const TextStyle(
                        fontSize: 15, fontStyle: FontStyle.normal))),
            outlinedButtonTheme: OutlinedButtonThemeData(
                style: OutlinedButton.styleFrom(
              side: BorderSide(width: 3.0, color: ColorsUtils.customYellow),
              padding:
                  const EdgeInsets.symmetric(horizontal: 30.0, vertical: 6),
              textStyle: TextStyle(
                color: ColorsUtils.customBlackColor,
                fontSize: 15,
              ),
            )),
            appBarTheme: AppBarTheme(
              backgroundColor: ColorsUtils.customBlack,
              foregroundColor: ColorsUtils.customYellow,
              iconTheme: IconThemeData(
                color: ColorsUtils.customYellow,
              ),
              actionsIconTheme: IconThemeData(
                color: ColorsUtils.customYellow,
              ),
            )),
        home: HomeScreenMain(),
        routes: {
          HomeScreenMain.routeName: (ctx) => HomeScreenMain(),
          // UserLanguageLevelScreen.routeName: (ctx) =>
          //     const UserLanguageLevelScreen(),
          UserSettingsScreen.routeName: (ctx) => const UserSettingsScreen(),
          UserLessonsHistoryScreen.routeName: (ctx) =>
              const UserLessonsHistoryScreen(),
          UserLessonsResultsScreen.routeName: (ctx) =>
              const UserLessonsResultsScreen(),
        },
      ),
    );
  }
}
