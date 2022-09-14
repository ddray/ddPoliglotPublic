import 'package:auto_size_text/auto_size_text.dart';
import 'package:ddpoliglot_v6_flutter/utils/colors_utils.dart';
import 'package:flutter/material.dart';

class QuestButtonWidget extends StatelessWidget {
  const QuestButtonWidget(this.text, this.onPressed,
      {this.assetImageName = '', this.subtitle = '', Key? key})
      : super(key: key);
  final String text;
  final String subtitle;
  final String assetImageName;
  final Function onPressed;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 5),
      child: Card(
        elevation: 5,
        shape: OutlineInputBorder(
            borderRadius: BorderRadius.circular(20),
            borderSide: BorderSide(color: ColorsUtils.customYellow, width: 2)),
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: 15),
          //width: double.infinity,
          child: InkWell(
            onTap: () {
              onPressed();
            },
            child: ListTile(
              leading: assetImageName.isNotEmpty
                  ? Image.asset(assetImageName)
                  : null,
              title: AutoSizeText(
                text,
                style: TextStyle(
                  fontSize: 20,
                  color: ColorsUtils.customBlackColor,
                  fontWeight: FontWeight.bold,
                ),
                // textAlign: TextAlign.center,
                maxLines: 1,
              ),
              subtitle: AutoSizeText(
                subtitle,
                style: TextStyle(
                  fontSize: 15,
                  color: ColorsUtils.customBlackColor,
                ),
                // textAlign: TextAlign.center,
                maxLines: 2,
              ),
              trailing: Icon(
                Icons.chevron_right_sharp,
                size: 30,
                color: ColorsUtils.customYellowColor,
              ),
            ),
          ),
        ),
      ),
    );
  }

  // @override
  // Widget build(BuildContext context) {
  //   return Container(
  //     padding: const EdgeInsets.only(top: 10),
  //     width: double.infinity,
  //     child: ElevatedButton(
  //       child: ListTile(
  //         title: Text(text),
  //         subtitle: Text(subtitle),
  //       ),
  //       onPressed: () => onPressed(),
  //     ),
  //   );
  // }
}
