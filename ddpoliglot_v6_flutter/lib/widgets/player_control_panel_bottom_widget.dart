import 'package:ddpoliglot_v6_flutter/utils/colors_utils.dart';

import '../models/player_classes.dart';
import '../providers/playlist_provider.dart/play_list_provider.dart';
import 'package:flutter/material.dart';

class PlayerControlPanelBottomWidget extends StatelessWidget {
  const PlayerControlPanelBottomWidget({
    Key? key,
    required this.playListProvider,
    required this.onStartPlay,
    required this.onStopPlay,
  }) : super(key: key);

  final PlayListProvider playListProvider;
  final Function() onStartPlay;
  final Function() onStopPlay;

  @override
  Widget build(BuildContext context) {
    final Color iconColor = ColorsUtils.customYellow;
    return Align(
      alignment: Alignment.bottomCenter,
      child: FractionallySizedBox(
        widthFactor: 1,
        child: Container(
          height: 100,
          decoration: BoxDecoration(
            gradient: LinearGradient(
              colors: [
                const Color.fromRGBO(255, 255, 255, 1),
                const Color.fromRGBO(220, 220, 220, 1),
                const Color.fromRGBO(76, 76, 105, 1).withOpacity(1),
                const Color.fromRGBO(26, 26, 55, 1).withOpacity(1),
              ],
              begin: Alignment.topCenter,
              end: Alignment.bottomCenter,
              stops: const [0, .1, .5, 1],
            ),
          ),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.start,
            children: [
              if (playListProvider.itemsCount > 0)
                Slider(
                    min: 0,
                    max: playListProvider.itemsCount > 1
                        ? playListProvider.itemsCount - 1
                        : 1,
                    value: playListProvider.currentItemIndex,
                    onChanged: (value) async =>
                        {await playListProvider.goTo(value)}),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  IconButton(
                    icon: Icon(
                      Icons.cleaning_services,
                      color: iconColor,
                    ),
                    onPressed: () {},
                  ),
                  Row(
                    children: [
                      IconButton(
                        iconSize: 30,
                        onPressed: () => playListProvider.goPrev(),
                        icon: Icon(
                          Icons.arrow_left,
                          color: iconColor,
                        ),
                        splashColor: Colors.white,
                      ),
                      playListProvider.playerState ==
                                  AudioSPlayerState.stoped ||
                              playListProvider.playerState ==
                                  AudioSPlayerState.paused
                          ? IconButton(
                              iconSize: 30,
                              icon: Icon(
                                Icons.play_circle_outline,
                                color: iconColor,
                              ),
                              onPressed: () async {
                                onStartPlay();
                                await playListProvider.playerPlay();
                              },
                            )
                          : playListProvider.playerState ==
                                  AudioSPlayerState.plaing
                              ? IconButton(
                                  iconSize: 30,
                                  icon: Icon(
                                    Icons.pause_circle,
                                    color: iconColor,
                                  ),
                                  onPressed: () async {
                                    await playListProvider.playerStop();
                                    onStopPlay();
                                  })
                              : IconButton(
                                  iconSize: 30,
                                  icon: Icon(
                                    Icons.ac_unit,
                                    color: iconColor,
                                  ),
                                  onPressed: () => {}),
                      IconButton(
                        iconSize: 30,
                        icon: Icon(
                          Icons.arrow_right,
                          color: iconColor,
                        ),
                        onPressed: () => playListProvider.goNext(),
                      ),
                    ],
                  ),
                  IconButton(
                    icon: Icon(
                      Icons.stop_screen_share_outlined,
                      color: iconColor,
                    ),
                    onPressed: () async =>
                        await playListProvider.finishLesson(),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
