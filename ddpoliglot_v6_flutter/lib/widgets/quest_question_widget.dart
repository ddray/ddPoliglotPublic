import 'package:auto_size_text/auto_size_text.dart';
import 'package:ddpoliglot_v6_flutter/utils/colors_utils.dart';
import 'package:flutter/material.dart';

class QuestQuestionWidget extends StatelessWidget {
  const QuestQuestionWidget(
    this.text, {
    this.subtitle = '',
    Key? key,
  }) : super(key: key);
  final String text;
  final String subtitle;

  @override
  Widget build(BuildContext context) {
    return Container(
      // color: Colors.green,
      alignment: Alignment.center,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.center,
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          AutoSizeText(
            text,
            style: TextStyle(
              fontSize: 25,
              color: ColorsUtils.customBlackColor,
            ),
            textAlign: TextAlign.center,
            maxLines: 1,
          ),

          if (subtitle.isNotEmpty)
            ListTile(
              title: AutoSizeText(
                subtitle,
                style: TextStyle(
                  fontSize: 18,
                  color: ColorsUtils.customBlackColor,
                ),
                textAlign: TextAlign.center,
                maxLines: 2,
              ),
            ),
          // Text(
          //   subtitle,
          //   style: const TextStyle(fontSize: 15),
          //   textAlign: TextAlign.center,
          // ),
          const SizedBox(
            height: 10,
          ),
        ],
      ),
    );
  }
}
