import '../models/screen_item.dart';
import '../providers/playlist_provider.dart/play_list_provider.dart';
import '../widgets/player_display_widget.dart';
import 'package:flutter/material.dart';

class PlayerDisplayWrapperWidget extends StatelessWidget {
  const PlayerDisplayWrapperWidget({
    Key? key,
    required this.playListProvider,
    required this.screenItem,
    required this.textFirst,
    required this.onTap,
  }) : super(key: key);

  final PlayListProvider playListProvider;
  final ScreenItem screenItem;
  final bool textFirst;
  final Function() onTap;

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: () => onTap(),
      child: Container(
        color: Colors.white,
        child:
            PlayerDisplayWidget(screenItem: screenItem, textFirst: textFirst),
      ),
    );
  }
}
