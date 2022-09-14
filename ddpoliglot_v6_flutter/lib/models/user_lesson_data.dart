import 'package:ddpoliglot_v6_flutter/models/data/Word.dart';
import 'package:ddpoliglot_v6_flutter/models/data/user_lesson.dart';
import 'package:ddpoliglot_v6_flutter/models/play_list.dart';
import 'package:ddpoliglot_v6_flutter/models/play_list_item.dart';

// ignore_for_file: file_names
import 'package:json_annotation/json_annotation.dart';

part 'user_lesson_data.g.dart';

@JsonSerializable()
class UserLessonData {
  bool finished = false;
  List<PlayList>? playLists = [];
  UserLesson? userLesson;
  int currentPlayListIndex;
  int currentItemNum;
  DateTime? startPlay;
  int runLessonNum;

  UserLessonData(
      {this.userLesson,
      this.playLists,
      this.startPlay,
      this.finished = false,
      this.currentPlayListIndex = 0,
      this.currentItemNum = 0,
      this.runLessonNum = 0});

  factory UserLessonData.fromJson(Map<String, dynamic> data) =>
      _$UserLessonDataFromJson(data);

  Map<String, dynamic> toJson() => _$UserLessonDataToJson(this);

  List<PlayList> getPlayLists() {
    return playLists ?? [];
  }

  PlayListItem? getCurrentItem() {
    if ((playLists ?? []).isEmpty) {
      return null;
    }

    return playLists?[currentPlayListIndex].items[currentItemNum];
  }

  double getItemsCount() {
    if ((playLists ?? []).isEmpty) {
      return 0;
    }

    return playLists![currentPlayListIndex].items.length.toDouble();
  }

  Future<void> goTo(double index) async {
    if (playLists!.isEmpty) return;
    if (playLists![currentPlayListIndex].items.length <= index) {
      currentItemNum = 0;
    } else {
      currentItemNum = index.toInt();
    }
  }

  List<Word> getWords() {
    return userLesson == null
        ? []
        : userLesson!.userLessonWords
            .where((x) => x.wordType == 0)
            .map((e) => e.word!)
            .toList();
  }

  List<Word> getWordsForRepetition() {
    return userLesson == null
        ? []
        : userLesson!.userLessonWords
            .where((x) => x.wordType == 1)
            .map((e) => e.word!)
            .toList();
  }
}
