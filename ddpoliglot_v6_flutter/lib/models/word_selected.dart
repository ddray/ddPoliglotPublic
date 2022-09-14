import 'package:json_annotation/json_annotation.dart';

import 'data/Word.dart';
import 'data/WordPhrase.dart';

part 'word_selected.g.dart';

@JsonSerializable()
class WordSelected {
  Word word;
  List<WordPhrase> phraseWords;
  List<WordPhrase> phraseWordsSelected;

  WordSelected(
    this.word,
    this.phraseWords,
    this.phraseWordsSelected,
  );

  factory WordSelected.fromJson(Map<String, dynamic> data) =>
      _$WordSelectedFromJson(data);

  Map<String, dynamic> toJson() => _$WordSelectedToJson(this);
}
