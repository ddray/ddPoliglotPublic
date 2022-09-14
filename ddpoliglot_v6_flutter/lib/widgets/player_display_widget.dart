import '../models/screen_item.dart';
import 'package:flutter/material.dart';

// class PlayerDisplayWidget extends StatelessWidget {
//   const PlayerDisplayWidget({
//     Key? key,
//     required this.screenItem,
//     required this.textFirst,
//   }) : super(key: key);

//   final ScreenItem screenItem;
//   final bool textFirst;

//   @override
//   Widget build(BuildContext context) {
//     return Center(
//       child: Column(
//         children: [
//           if (screenItem.section1.visible)
//             Expanded(
//                 flex: screenItem.section1.flex,
//                 child: Container(
//                   color: Colors.lightBlue[50],
//                   padding: const EdgeInsets.fromLTRB(5, 0, 5, 0),
//                   width: double.infinity,
//                   child: Align(
//                       alignment: screenItem.section1.alignment,
//                       child: Text(
//                         textFirst
//                             ? screenItem.videoCurrentText
//                             : screenItem.videoCurrentTranslationText,
//                         style: TextStyle(
//                             fontFamily: 'Helvetica',
//                             fontSize: screenItem.section1.fontSize,
//                             color: screenItem.section1.color),
//                         textAlign: TextAlign.center,
//                       )),
//                 )),
//           if (screenItem.section2.visible)
//             Expanded(
//                 flex: screenItem.section2.flex,
//                 child: Container(
//                   color: Colors.lightGreen[50],
//                   padding: const EdgeInsets.fromLTRB(5, 0, 5, 0),
//                   width: double.infinity,
//                   child: Align(
//                       alignment: screenItem.section2.alignment,
//                       child: Text(
//                         textFirst
//                             ? screenItem.videoCurrentPronText
//                             : screenItem.videoCurrentText,
//                         style: TextStyle(
//                             fontFamily:
//                                 textFirst ? 'SourceSansPro' : 'Helvetica',
//                             fontSize: screenItem.section2.fontSize,
//                             color: screenItem.section2.color),
//                         textAlign: TextAlign.center,
//                       )),
//                 )),
//           if (screenItem.section3.visible)
//             Expanded(
//                 flex: screenItem.section3.flex,
//                 child: Container(
// //                    color: Colors.brown[50],
//                   color: Colors.white,
//                   padding: const EdgeInsets.fromLTRB(5, 0, 5, 0),
//                   width: double.infinity,
//                   child: Align(
//                       alignment: screenItem.section3.alignment,
//                       child: Text(
//                         textFirst
//                             ? screenItem.videoCurrentTranslationText
//                             : screenItem.videoCurrentPronText,
//                         style: TextStyle(
//                             fontFamily:
//                                 textFirst ? 'Helvetica' : 'SourceSansPro',
//                             fontSize: screenItem.section3.fontSize,
//                             color: screenItem.section3.color),
//                         textAlign: TextAlign.center,
//                       )),
//                 )),
//         ],
//       ),
//     );
//   }
// }

class PlayerDisplayWidget extends StatelessWidget {
  const PlayerDisplayWidget({
    Key? key,
    required this.screenItem,
    required this.textFirst,
  }) : super(key: key);

  final ScreenItem screenItem;
  final bool textFirst;

  @override
  Widget build(BuildContext context) {
    return Center(
      child: AspectRatio(
        aspectRatio: 16 / 9,
        child: Column(
          children: [
            if (screenItem.section1.visible)
              Expanded(
                  flex: screenItem.section1.flex,
                  child: Container(
//                    color: Colors.lightBlue[50],
                    color: Colors.white,
                    padding: const EdgeInsets.fromLTRB(5, 0, 5, 0),
                    width: double.infinity,
                    child: Align(
                        alignment: screenItem.section1.alignment,
                        child: Text(
                          textFirst
                              ? screenItem.videoCurrentText
                              : screenItem.videoCurrentTranslationText,
                          style: TextStyle(
                              fontFamily: 'Helvetica',
                              fontSize: screenItem.section1.fontSize,
                              color: screenItem.section1.color),
                          textAlign: TextAlign.center,
                        )),
                  )),
            if (screenItem.section2.visible)
              Expanded(
                  flex: screenItem.section2.flex,
                  child: Container(
//                    color: Colors.lightGreen[50],
                    color: Colors.white,
                    padding: const EdgeInsets.fromLTRB(5, 0, 5, 0),
                    width: double.infinity,
                    child: Align(
                        alignment: screenItem.section2.alignment,
                        child: Text(
                          textFirst
                              ? screenItem.videoCurrentPronText
                              : screenItem.videoCurrentText,
                          style: TextStyle(
                              fontFamily:
                                  textFirst ? 'SourceSansPro' : 'Helvetica',
                              fontSize: screenItem.section2.fontSize,
                              color: screenItem.section2.color),
                          textAlign: TextAlign.center,
                        )),
                  )),
            if (screenItem.section3.visible)
              Expanded(
                  flex: screenItem.section3.flex,
                  child: Container(
//                    color: Colors.brown[50],
                    color: Colors.white,
                    padding: const EdgeInsets.fromLTRB(5, 0, 5, 0),
                    width: double.infinity,
                    child: Align(
                        alignment: screenItem.section3.alignment,
                        child: Text(
                          textFirst
                              ? screenItem.videoCurrentTranslationText
                              : screenItem.videoCurrentPronText,
                          style: TextStyle(
                              fontFamily:
                                  textFirst ? 'Helvetica' : 'SourceSansPro',
                              fontSize: screenItem.section3.fontSize,
                              color: screenItem.section3.color),
                          textAlign: TextAlign.center,
                        )),
                  )),
          ],
        ),
      ),
    );
  }
}
