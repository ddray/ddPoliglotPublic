// ignore_for_file: file_names
import 'Word.dart';
import './WordPhrase.dart';

import 'package:json_annotation/json_annotation.dart';

part 'WordPhraseWord.g.dart';

@JsonSerializable()
class WordPhraseWord {
  int wordPhraseWordID;
  int wordID;
  Word? word;
  int wordPhraseID;
  WordPhrase? wordPhrase;
  int phraseOrder;

  WordPhraseWord(
    this.wordPhraseWordID,
    this.wordID,
    this.word,
    this.wordPhraseID,
    this.wordPhrase,
    this.phraseOrder,
  );

  factory WordPhraseWord.fromJson(Map<String, dynamic> data) =>
      _$WordPhraseWordFromJson(data);

  Map<String, dynamic> toJson() => _$WordPhraseWordToJson(this);
}
