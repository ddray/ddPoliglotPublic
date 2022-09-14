import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'dart:async';

import '../models/screen_item.dart';
import '../widgets/player_control_panel_top_widget.dart';
import '../widgets/navigation_drawer_widget.dart';
import '../widgets/player_control_panel_bottom_widget.dart';
import '../widgets/player_display_wrapper_widget.dart';
import '../models/player_classes.dart';
import '../providers/playlist_provider.dart/play_list_provider.dart';

class PlayerScreen extends StatefulWidget {
  const PlayerScreen({Key? key}) : super(key: key);

  @override
  State<PlayerScreen> createState() => _PlayerScreenState();
}

class _PlayerScreenState extends State<PlayerScreen> {
  late bool showControlPanels;
  Timer? _timer; // timer to hide player control panel

  void startHideControlPanelsPending() {
    stopHideControlPanelsPending();
    _timer = Timer(Duration(seconds: 7.toInt()), () {
      setState(() {
        showControlPanels = false;
      });
    });
  }

  void stopHideControlPanelsPending() {
    if (_timer != null) {
      _timer!.cancel();
    }
  }

  @override
  void initState() {
    super.initState();
    showControlPanels = true;
  }

  @override
  Widget build(BuildContext context) {
    final playListProvider =
        Provider.of<PlayListProvider>(context, listen: true);
    final ScreenItem? screenItem = playListProvider.currentItem != null
        ? ScreenItem.fromPlayListItem(playListProvider.currentItem!)
        : null;
    final bool textFirst = playListProvider.currentItem != null
        ? playListProvider.currentItem!.activityType ==
            PlayListActivityTypes.textFirst.index
        : false;

    debugPrint(
        'home refresh: loading: ${playListProvider.isloading}, curr:${playListProvider.currentItem}, screenItem:$screenItem ');

    return Scaffold(
      drawer: NavigationDrawer(
        pageType: Pages.playerPage.index,
      ),
      body: Container(
        child: playListProvider.isloading || screenItem == null
            ? Container(
                padding: const EdgeInsets.only(top: 70),
                child: const Center(child: CircularProgressIndicator()),
              )
            : Stack(
                children: [
                  PlayerDisplayWrapperWidget(
                      playListProvider: playListProvider,
                      screenItem: screenItem,
                      textFirst: textFirst,
                      onTap: () {
                        final res = !showControlPanels;
                        if (res &&
                            playListProvider.playerState ==
                                AudioSPlayerState.plaing) {
                          startHideControlPanelsPending();
                        } else {
                          stopHideControlPanelsPending();
                        }

                        setState(() {
                          showControlPanels = res;
                        });
                      }),
                  if (showControlPanels)
                    PlayerControlPanelBottomWidget(
                      playListProvider: playListProvider,
                      onStartPlay: () {
                        startHideControlPanelsPending();
                      },
                      onStopPlay: () {
                        stopHideControlPanelsPending();
                      },
                    ),
                  if (showControlPanels) const PlayerControlPanelTopWidget()
                ],
              ),
      ),
    );
  }

  @override
  void dispose() {
    if (_timer != null) {
      _timer!.cancel();
    }
    super.dispose();
  }
}
