// ignore_for_file: constant_identifier_names

import 'package:ddpoliglot_v6_flutter/models/play_list_item.dart';
import 'package:json_annotation/json_annotation.dart';

@JsonSerializable()
class Plailable {
  @JsonKey(ignore: true)
  PlayListItem? playListItem; // item with audio object
  @JsonKey(ignore: true)
  PlayListItem? playListItemSpeed1;
  @JsonKey(ignore: true)
  PlayListItem? playListItemSpeed2;
}

class PlailablePare {
  late Plailable? source;
  late Plailable? translation;
  PlailablePare({this.source, this.translation});
}

enum PlayListItemTypes {
  word,
  wordSpeed1,
  wordSpeed2,
  wordTranslation,
  wordPhrase,
  wordPhraseSpeed1,
  wordPhraseSpeed2,
  wordPhraseTranslation,
  dictorPhrase,
  dialogOrWordPhrase,
}

enum LessonTypes { normal, short, huge, testShort, testFull }

enum ChildrenType {
  no,
  orderRpt,
  orderRpt2,
  orderRpt3,
  orderRptTr,
  orderRpt2Tr,
  orderRpt3Tr,
  rnd,
  rndRepeatWrd,
  rndRepeatPhrases,
  rndTr,
  rndRepeatWrdTr,
  rndRepeatPhrasesTr,
}

enum ScreenTextTypes { Text, Pron, Transl }

class ScreenClass {
  late String level0;
  late String level1;
  late String level2;
}

class ScreenTextWithClass {
  late String value;
  late ScreenClass screenClass;
}

class CurrentScreen {
  late CurrentScreenTypes currentScreenType;
  late PlayListActivityTypes activityType;
  late ScreenTextWithClass text;
  late ScreenTextWithClass pron;
  late ScreenTextWithClass transl;
}

enum CurrentScreenTypes {
  textYesPronYesTranslNoTextFirst,
  textYesPronYesTranslYesTranslFirst,
  textYesPronYesTranslYesTextFirst,
  textNoPronNoTranslYesTranslFirst,
  textYesTranslNoTextFirst,
  textYesTranslYesTranslFirst,
  textYesTranslYesTextFirst,
  textNoTranslYesTranslFirst,
  dictorTextFirst,
}

enum PlayListActivityTypes {
  textFirst,
  translationFirst,
}

enum TextLen { Short, Middle, Long }

enum AudioSPlayerState {
  stoped,
  plaing,
  paused,
}
