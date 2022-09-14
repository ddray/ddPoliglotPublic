import 'dart:convert';

import 'package:json_annotation/json_annotation.dart';

import 'data/MixParam.dart';
part 'article_by_param_data.g.dart';

@JsonSerializable()
class ArticleByParamData {
  //final List<WordSelected> selectedWords;
  final String? dialogText;
  final String? phrasesText;
  final String? phrasesTranslationText;
  final List<MixParam> mixParamsList;
  final String? baseName;
  final String? firstDictorPhrases;
  final String? beforeFinishDictorPhrases;
  final String? finishDictorPhrases;
  // final List<Word> wordsToRepeat;
  // final List<WordPhrase> wordPhrasesToRepeat;
  final int? maxWordsToRepeatForGeneration;

  ArticleByParamData(
    // this.selectedWords,
    this.dialogText,
    this.phrasesText,
    this.phrasesTranslationText,
    this.mixParamsList,
    this.baseName,
    this.firstDictorPhrases,
    this.beforeFinishDictorPhrases,
    this.finishDictorPhrases,
    // this.wordsToRepeat,
    // this.wordPhrasesToRepeat,
    this.maxWordsToRepeatForGeneration,
  );

  factory ArticleByParamData.fromJson(Map<String, dynamic> data) =>
      _$ArticleByParamDataFromJson(data);

  Map<String, dynamic> toJson() => _$ArticleByParamDataToJson(this);

  ArticleByParamData clone() {
    return ArticleByParamData(
//      selectedWords,
      dialogText,
      phrasesText,
      phrasesTranslationText,
      mixParamsList,
      baseName,
      firstDictorPhrases,
      beforeFinishDictorPhrases,
      finishDictorPhrases,
      // wordsToRepeat,
      // wordPhrasesToRepeat,
      maxWordsToRepeatForGeneration,
    );
  }

  factory ArticleByParamData.fromDb(Map<String, dynamic> row) {
    final s = row['jsonData'];
    final item = json.decode(s) as Map<String, dynamic>;
    return ArticleByParamData.fromJson(item);
  }

  Map<String, dynamic> toDb(int lessonType) {
    final js = toJson();
    final jsonS = json.encode(js);
    return {
      'lessonType': lessonType,
      'jsonData': jsonS,
    };
  }

  ArticleByParamData cloneWithCloneDictorPhrases() {
    ArticleByParamData articleByParamData = ArticleByParamData(
      dialogText,
      phrasesText,
      phrasesTranslationText,
      [
        ...mixParamsList.map((e) => MixParam(
              e.mixItems,
              e.trFirst,
              e.active,
              e.trActive,
              e.phrasesMixType,
              e.repeat,
              e.trRepeat,
              e.repeatOrder,
              e.trRepeatOrder,
              e.addSlowInRepeatOrder,
              e.addSlow2InRepeatOrder,
              e.trAddSlowInRepeatOrder,
              e.trAddSlow2InRepeatOrder,
              e.repeatBaseWord,
              e.trRepeatBaseWord,
              e.firstDictorPhrasesWithAudioW,
              e.firstBeforeDialogDictorPhrasesWithAudioW,
              e.beforeByOrderMixDictorPhrasesWithAudioW,
              e.insideByOrderMixDictorPhrasesWithAudioW,
              e.beforeBaseWordsDirMixDictorPhrasesWithAudioW,
              e.insideBaseWordsDirMixDictorPhrasesWithAudioW,
              e.beforeBaseWordsRevMixDictorPhrasesWithAudioW,
              e.insideBaseWordsRevMixDictorPhrasesWithAudioW,
              e.beforeAllDirMixDictorPhrasesWithAudioW,
              e.insideAllDirMixDictorPhrasesWithAudioW,
              e.beforeAllRevMixDictorPhrasesWithAudioW,
              e.insideAllRevMixDictorPhrasesWithAudioW,
              e.beforeFinishDictorPhrasesWithAudioW,
              e.finishDictorPhrasesWithAudioW,
            ))
      ],
      baseName,
      firstDictorPhrases,
      beforeFinishDictorPhrases,
      finishDictorPhrases,
      maxWordsToRepeatForGeneration,
    );

    return articleByParamData;
  }
}
