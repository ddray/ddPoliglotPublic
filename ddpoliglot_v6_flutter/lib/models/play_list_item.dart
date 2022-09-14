// ignore_for_file: file_names
import 'package:json_annotation/json_annotation.dart';

part 'play_list_item.g.dart';

@JsonSerializable()
class PlayListItem {
  late int itemID;
  late int itemParentID;
  late Object?
      itemRef; // Word | WordPhrase | WordTranslation | WordPhraseTranslation
  late Object? itemParentRef; //Word | WordPhrase
  late String text;
  late String pronunciation;
  late String textSpeechFileName;
  late int speachDuration;
  late int pause;
  late int itemType; // PlayListItemTypes..
  late int activityType; // PlayListActivityTypes
  late int? order;
  late bool? isCurrent;
  late int? fromSecond;
  late int childrenType;
  late int srcWordsQty;

  PlayListItem(
      {this.itemID = 0,
      this.itemParentID = 0,
      this.itemRef,
      this.itemParentRef,
      this.text = "",
      this.pronunciation = "",
      this.textSpeechFileName = "",
      this.speachDuration = 0,
      this.pause = 0,
      this.itemType = -1,
      this.activityType = 0,
      this.order,
      this.isCurrent,
      this.fromSecond,
      this.childrenType = 0,
      this.srcWordsQty = -1});

  PlayListItem.fromOther(PlayListItem other) {
    itemID = other.itemID;
    itemParentID = other.itemParentID;
    itemRef = other.itemRef;
    itemParentRef = other.itemParentRef;
    text = other.text;
    pronunciation = other.pronunciation;
    textSpeechFileName = other.textSpeechFileName;
    speachDuration = other.speachDuration;
    pause = other.pause;
    itemType = other.itemType;
    activityType = other.activityType;
    order = other.order;
    isCurrent = other.isCurrent;
    fromSecond = other.fromSecond;
    childrenType = other.childrenType;
    srcWordsQty = other.srcWordsQty;
  }

  factory PlayListItem.fromJson(Map<String, dynamic> data) =>
      _$PlayListItemFromJson(data);

  Map<String, dynamic> toJson() => _$PlayListItemToJson(this);
}
