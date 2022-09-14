import 'package:ddpoliglot_v6_flutter/providers/user_settings/user_settings_provider.dart';
import 'package:ddpoliglot_v6_flutter/screens/refresh_database_screen.dart';
import 'package:ddpoliglot_v6_flutter/screens/user_lesson_start_wizard_screen.dart';
import 'package:ddpoliglot_v6_flutter/screens/user_lessons_history_screen.dart';
import 'package:ddpoliglot_v6_flutter/screens/user_lessons_results_screen.dart';
import 'package:ddpoliglot_v6_flutter/screens/user_lesson_finish_screen.dart';
import 'package:ddpoliglot_v6_flutter/screens/player_screen.dart';
import 'package:ddpoliglot_v6_flutter/screens/splash_screen.dart';
import 'package:ddpoliglot_v6_flutter/screens/user_settings_wizard_screen.dart';
// import 'package:ddpoliglot_v6_flutter/screens/user_settings_screen.dart';
import '../providers/auth/auth_provider.dart';
import '../providers/playlist_provider.dart/play_list_provider.dart';
import 'auth_screen.dart';

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

class HomeScreenMain extends StatelessWidget {
  HomeScreenMain({Key? key}) : super(key: key) {
    debugPrint('create HomeScreenMain');
  }

  static const routeName = '/home';

  @override
  Widget build(BuildContext context) {
    final authProvider = context.watch<AuthProvider>();
    final UserSettingsProvider userSettingsProvider =
        context.watch<UserSettingsProvider>();
    final isSettings = !userSettingsProvider.state.isEmpty;
    final isUserLanguageLevel =
        (userSettingsProvider.state.userLanguageLevel?.level != null);
    final playListDataProvider =
        Provider.of<PlayListProvider>(context, listen: true);
    final isDatabaseRefreshed =
        (playListDataProvider.databaseState?.dictWords ?? []).isNotEmpty;
    final isPlayList = (playListDataProvider.playLists ?? []).isNotEmpty &&
        (playListDataProvider.playLists?[0].items ?? []).isNotEmpty;
    final isFinished = (playListDataProvider).finished;
    final playListPeparationStep = (playListDataProvider).state.preparationStep;
    final runLessonNum =
        playListDataProvider.state.userLessonData?.runLessonNum ?? 0;

    debugPrint(
        '!!!!!! rebuild HomeScreen _isAuth:${authProvider.state.isAuth}, isSettings:$isSettings, isUserLanguageLevel:$isUserLanguageLevel, isDatabaseRefreshed: $isDatabaseRefreshed, isPlayList:$isPlayList, isFinished:$isFinished, playListPeparationStep: $playListPeparationStep');

    return !authProvider.state.isAuth
        ? FutureBuilder(
            future: authProvider.tryLoadFromStore(),
            builder: (ctx, snapshot) {
              if (snapshot.hasError) {
                throw Exception(
                    snapshot.error.toString() + snapshot.stackTrace.toString());
              } else if (snapshot.connectionState == ConnectionState.waiting) {
                return const SplashScreen();
              } else {
                return const AuthScreen();
              }
            })
        : authProvider.state.isNeedAuthAnonim
            ? const AuthScreen() // current anonim want to create account
            : !isSettings
                ? FutureBuilder(
                    future: userSettingsProvider.tryLoadFromStore(),
                    builder: (ctx, snapshot) {
                      if (snapshot.hasError) {
                        throw Exception(snapshot.error.toString() +
                            snapshot.stackTrace.toString());
                      } else if (snapshot.connectionState ==
                          ConnectionState.waiting) {
                        return const SplashScreen();
                      } else {
                        return const UserSettingsWizardScreen();
                      }
                    })
                : !isDatabaseRefreshed
                    ? RefreshDatabaseScreen()
                    : !isPlayList
                        ? playListPeparationStep ==
                                PreparationStep.showUserData.index
                            ? const UserLessonsResultsScreen()
                            : playListPeparationStep ==
                                    PreparationStep.showLessonsList.index
                                ? const UserLessonsHistoryScreen()
                                : playListPeparationStep ==
                                        PreparationStep.configureLesson.index
                                    ? runLessonNum > 0
                                        ? UserLessonStartWizardScreen(
                                            runLessonNum: runLessonNum,
                                          )
                                        : const UserLessonStartWizardScreen()
                                    : const SplashScreen()
                        : !isFinished
                            ? const PlayerScreen()
                            : const UserLessonFinishScreen();
  }
}
