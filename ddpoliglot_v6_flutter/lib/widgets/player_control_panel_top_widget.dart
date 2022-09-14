import 'package:ddpoliglot_v6_flutter/utils/colors_utils.dart';

import '../providers/playlist_provider.dart/play_list_provider.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

class PlayerControlPanelTopWidget extends StatelessWidget {
  const PlayerControlPanelTopWidget({
    Key? key,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final playListProvider =
        Provider.of<PlayListProvider>(context, listen: true);
    final lessonNum = playListProvider.state.userLessonData!.userLesson!.num;
    final playlistNum =
        playListProvider.state.userLessonData!.currentPlayListIndex + 1;
    return Align(
      alignment: Alignment.topCenter,
      child: FractionallySizedBox(
        widthFactor: 1,
        child: Container(
          height: 80,
          decoration: BoxDecoration(
            gradient: LinearGradient(
              colors: [
                const Color.fromRGBO(255, 255, 255, 1),
                const Color.fromRGBO(176, 176, 205, 1),
                const Color.fromRGBO(26, 26, 55, 1).withOpacity(1),
              ],
              begin: Alignment.bottomCenter,
              end: Alignment.topCenter,
              stops: const [0, .2, 1],
            ),
          ),
          padding: const EdgeInsets.fromLTRB(3, 23, 0, 0),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.start,
                children: [
                  IconButton(
                    onPressed: () async {
                      Scaffold.of(context).openDrawer();
                      await playListProvider.playerStop();
                    },
                    icon: const Icon(Icons.menu),
                    color: ColorsUtils.customYellow,
                  ),
                  Container(
                    margin: const EdgeInsets.fromLTRB(15, 2, 0, 0),
                    child: Text(
                      'Урок $lessonNum / $playlistNum.',
                      style: TextStyle(
                          color: ColorsUtils.customYellowColor, fontSize: 20),
                    ),
                  )
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
