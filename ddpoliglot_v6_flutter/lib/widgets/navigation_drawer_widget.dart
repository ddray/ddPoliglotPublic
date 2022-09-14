import 'package:ddpoliglot_v6_flutter/providers/database/database_provider.dart';
import 'package:ddpoliglot_v6_flutter/providers/playlist_provider.dart/play_list_provider.dart';
import 'package:ddpoliglot_v6_flutter/screens/user_lessons_history_screen.dart';
import 'package:ddpoliglot_v6_flutter/screens/user_lessons_results_screen.dart';
import 'package:ddpoliglot_v6_flutter/screens/user_settings_screen.dart';
import 'package:ddpoliglot_v6_flutter/utils/colors_utils.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../providers/auth/auth_provider.dart';

class NavigationDrawer extends StatelessWidget {
  const NavigationDrawer({Key? key, this.pageType = 0}) : super(key: key);
  final int pageType;
  @override
  Widget build(BuildContext context) {
    final playListProvider =
        Provider.of<PlayListProvider>(context, listen: true);
    final auth = Provider.of<AuthProvider>(context, listen: false);
    final databaseProvider =
        Provider.of<DatabaseProvider>(context, listen: false);
    final isLessons = (databaseProvider.state.userLessons ?? []).isNotEmpty;

    return Drawer(
      child: ListView(
        padding: EdgeInsets.zero,
        children: [
          DrawerHeader(
            decoration: BoxDecoration(
              color: ColorsUtils.customBlack,
            ),
            child: const Image(
              image: AssetImage('DD_LOGO-01.png'),
            ),
          ),
          if ((playListProvider.playLists ?? []).isNotEmpty &&
              pageType == Pages.playerPage.index)
            for (var i = 0; i < playListProvider.playLists!.length; i++)
              createDrawerBodyItem(
                  icon: Icons.accessibility_rounded,
                  text: playListProvider.playLists![i].name,
                  onTap: () {
                    playListProvider.setCurrentPlayListIndex(i);
                    Navigator.of(context).pop();
                    // Navigator.of(context).pushNamed(HomeScreenMain.routeName);
                  },
                  selected: (playListProvider.currentPlayListIndex == i)),
          const Divider(),
          if (pageType != Pages.settingPage.index)
            createDrawerBodyItem(
                icon: Icons.settings,
                text: 'Настройки',
                onTap: () {
                  Navigator.of(context).pop();
                  Navigator.of(context)
                      .pushNamed(UserSettingsScreen.routeName, arguments: 1);
                }),
          if (pageType != Pages.resultsPage.index && isLessons)
            createDrawerBodyItem(
                icon: Icons.bar_chart,
                text: 'Мои результаты',
                onTap: () {
                  Navigator.of(context).pop();
                  Navigator.of(context).pushNamed(
                      UserLessonsResultsScreen.routeName,
                      arguments: 1);
                }),
          if (pageType != Pages.lessonsPage.index &&
              // pageType != Pages.playerPage.index &&
              isLessons)
            createDrawerBodyItem(
                icon: Icons.play_lesson_outlined,
                text: 'Мои уроки',
                onTap: () {
                  if (pageType != Pages.lessonWizardPage.index) {
                    Navigator.of(context).pop();
                    Navigator.of(context).pushNamed(
                        UserLessonsHistoryScreen.routeName,
                        arguments: 1);
                  } else {
                    Navigator.of(context).pop();
                    playListProvider.setPreparationStep(
                        PreparationStep.showLessonsList.index);
                    // Navigator.of(context)
                    //     .popAndPushNamed(UserLessonsHistoryScreen.routeName);
                  }
                }),
          // createDrawerBodyItem(
          //     icon: Icons.leaderboard,
          //     text: 'Set language level',
          //     onTap: () {
          //       Navigator.of(context).pop();
          //       Navigator.of(context)
          //           .pushNamed(UserLanguageLevelScreen.routeName, arguments: 1);
          //     }),
          createDrawerBodyItem(
              icon: Icons.logout,
              text: (auth.state.userName).startsWith("Anonim_")
                  ? 'Account'
                  : 'Logout',
              onTap: () async {
                (auth.state.userName).startsWith("Anonim_")
                    ? await auth.authAnonim()
                    : await auth.logout();
              }),
        ],
      ),
    );
  }

  Widget createDrawerBodyItem(
      {IconData? icon,
      required String text,
      GestureTapCallback? onTap,
      bool selected = false}) {
    return ListTile(
      title: Row(
        children: <Widget>[
          Icon(
            icon,
            color: ColorsUtils.customYellow,
          ),
          Padding(
            padding: const EdgeInsets.only(left: 8.0),
            child: Text(
              text,
              style: TextStyle(
                  fontWeight: (selected ? FontWeight.bold : FontWeight.normal),
                  color: ColorsUtils.customBlack),
            ),
          )
        ],
      ),
      onTap: (selected ? () {} : onTap),
    );
  }
}

enum Pages {
  homePage,
  playerPage,
  settingPage,
  resultsPage,
  lessonsPage,
  finishPage,
  lessonWizardPage,
}
