// ignore_for_file: file_names, non_constant_identifier_names
import 'package:ddpoliglot_v6_flutter/models/aPhWr.dart';

import './MixItem.dart';

import 'package:json_annotation/json_annotation.dart';

part 'MixParam.g.dart';

@JsonSerializable()
class MixParam {
  // int mixParamID;
  // String? articlePhraseKeyGuid;
  List<MixItem> mixItems;
  // int textHashCode;
  // int trTextHashCode;
  bool trFirst;
  bool active;
  // bool m0;
  // bool m0_repeat;
  // bool m01;
  // bool m01_repeat;
  // bool m012;
  // bool m012_repeat;
  // bool m0123;
  // bool m0123_repeat;
  bool trActive;
  // bool trM0;
  // bool trM0_repeat;
  // bool trM01;
  // bool trM01_repeat;
  // bool trM012;
  // bool trM012_repeat;
  // bool trM0123;
  // bool trM0123_repeat;
  // String? offer1;
  // String? offer2;
  // String? offer3;
  // String? offer4;
  // String? offer5;
  // String? trOffer1;
  // String? trOffer2;
  // String? trOffer3;
  // String? trOffer4;
  // String? trOffer5;
  int phrasesMixType; // mix-1 wave (for texts), mix-2 rnd (for words)
  int repeat; // use items in rnd repeat
  int trRepeat; // use items in rnd repeat
  int repeatOrder; // repeat in order for mix-2
  int trRepeatOrder; // repeat in order for mix-2
  bool addSlowInRepeatOrder; // speack slow in order for mix-2
  bool addSlow2InRepeatOrder; // speack slow in order for mix-2
  bool trAddSlowInRepeatOrder; // speack slow in order for mix-2
  bool trAddSlow2InRepeatOrder; // speack slow in order for mix-2
  int repeatBaseWord; // add rnd repeat of base words only for mix-2
  int trRepeatBaseWord; // add rnd repeat of base words only for mix-2
  // String? firstDictorPhrases;
  // String? firstBeforeDialogDictorPhrases;

  // String? beforeByOrderMixDictorPhrases;
  // String? insideByOrderMixDictorPhrases;

  // String? beforeBaseWordsDirMixDictorPhrases;
  // String? insideBaseWordsDirMixDictorPhrases;

  // String? beforeBaseWordsRevMixDictorPhrases;
  // String? insideBaseWordsRevMixDictorPhrases;

  // String? beforeAllDirMixDictorPhrases;
  // String? insideAllDirMixDictorPhrases;

  // String? beforeAllRevMixDictorPhrases;
  // String? insideAllRevMixDictorPhrases;

  // String? beforeFinishDictorPhrases;
  // String? finishDictorPhrases;

  // List<List<List<ArticlePhrase>>> firstDictorPhrasesWithAudio;
  // List<List<List<ArticlePhrase>>> firstBeforeDialogDictorPhrasesWithAudio;
  // List<List<List<ArticlePhrase>>> beforeByOrderMixDictorPhrasesWithAudio;
  // List<List<List<ArticlePhrase>>> insideByOrderMixDictorPhrasesWithAudio;
  // List<List<List<ArticlePhrase>>> beforeBaseWordsDirMixDictorPhrasesWithAudio;
  // List<List<List<ArticlePhrase>>> insideBaseWordsDirMixDictorPhrasesWithAudio;
  // List<List<List<ArticlePhrase>>> beforeBaseWordsRevMixDictorPhrasesWithAudio;
  // List<List<List<ArticlePhrase>>> insideBaseWordsRevMixDictorPhrasesWithAudio;
  // List<List<List<ArticlePhrase>>> beforeAllDirMixDictorPhrasesWithAudio;
  // List<List<List<ArticlePhrase>>> insideAllDirMixDictorPhrasesWithAudio;
  // List<List<List<ArticlePhrase>>> beforeAllRevMixDictorPhrasesWithAudio;
  // List<List<List<ArticlePhrase>>> insideAllRevMixDictorPhrasesWithAudio;
  // List<List<List<ArticlePhrase>>> beforeFinishDictorPhrasesWithAudio;
  // List<List<List<ArticlePhrase>>> finishDictorPhrasesWithAudio;

  List<List<List<APhWr>>> firstDictorPhrasesWithAudioW;
  List<List<List<APhWr>>> firstBeforeDialogDictorPhrasesWithAudioW;
  List<List<List<APhWr>>> beforeByOrderMixDictorPhrasesWithAudioW;
  List<List<List<APhWr>>> insideByOrderMixDictorPhrasesWithAudioW;
  List<List<List<APhWr>>> beforeBaseWordsDirMixDictorPhrasesWithAudioW;
  List<List<List<APhWr>>> insideBaseWordsDirMixDictorPhrasesWithAudioW;
  List<List<List<APhWr>>> beforeBaseWordsRevMixDictorPhrasesWithAudioW;
  List<List<List<APhWr>>> insideBaseWordsRevMixDictorPhrasesWithAudioW;
  List<List<List<APhWr>>> beforeAllDirMixDictorPhrasesWithAudioW;
  List<List<List<APhWr>>> insideAllDirMixDictorPhrasesWithAudioW;
  List<List<List<APhWr>>> beforeAllRevMixDictorPhrasesWithAudioW;
  List<List<List<APhWr>>> insideAllRevMixDictorPhrasesWithAudioW;
  List<List<List<APhWr>>> beforeFinishDictorPhrasesWithAudioW;
  List<List<List<APhWr>>> finishDictorPhrasesWithAudioW;

  // depend on lesson type
  @JsonKey(ignore: true)
  int repeatRepetitionsWordsInBaseMix = 2;

  @JsonKey(ignore: true)
  int repeatRepetitionsWordsAndPhrasesInAllMix = 2;

  @JsonKey(ignore: true)
  int wordPhasesInWord = 3;

  @JsonKey(ignore: true)
  bool repeatRepetitionsWordPhrases = true;

  @JsonKey(ignore: true)
  int lessonType = 0;

  MixParam(
    // this.mixParamID,
    // this.articlePhraseKeyGuid,
    this.mixItems,
    // this.textHashCode,
    // this.trTextHashCode,
    this.trFirst,
    this.active,
    // this.m0,
    // this.m0_repeat,
    // this.m01,
    // this.m01_repeat,
    // this.m012,
    // this.m012_repeat,
    // this.m0123,
    // this.m0123_repeat,
    this.trActive,
    // this.trM0,
    // this.trM0_repeat,
    // this.trM01,
    // this.trM01_repeat,
    // this.trM012,
    // this.trM012_repeat,
    // this.trM0123,
    // this.trM0123_repeat,
    // this.offer1,
    // this.offer2,
    // this.offer3,
    // this.offer4,
    // this.offer5,
    // this.trOffer1,
    // this.trOffer2,
    // this.trOffer3,
    // this.trOffer4,
    // this.trOffer5,
    this.phrasesMixType, // mix-1 wave (for texts), mix-2 rnd (for words)
    this.repeat, // use items in rnd repeat
    this.trRepeat, // use items in rnd repeat
    this.repeatOrder, // repeat in order for mix-2
    this.trRepeatOrder, // repeat in order for mix-2
    this.addSlowInRepeatOrder, // speack slow in order for mix-2
    this.addSlow2InRepeatOrder, // speack slow in order for mix-2
    this.trAddSlowInRepeatOrder, // speack slow in order for mix-2
    this.trAddSlow2InRepeatOrder, // speack slow in order for mix-2
    this.repeatBaseWord, // add rnd repeat of base words only for mix-2
    this.trRepeatBaseWord, // add rnd repeat of base words only for mix-2
    // this.firstDictorPhrases,
    // this.firstBeforeDialogDictorPhrases,
    // this.beforeByOrderMixDictorPhrases,
    // this.insideByOrderMixDictorPhrases,
    // this.beforeBaseWordsDirMixDictorPhrases,
    // this.insideBaseWordsDirMixDictorPhrases,
    // this.beforeBaseWordsRevMixDictorPhrases,
    // this.insideBaseWordsRevMixDictorPhrases,
    // this.beforeAllDirMixDictorPhrases,
    // this.insideAllDirMixDictorPhrases,
    // this.beforeAllRevMixDictorPhrases,
    // this.insideAllRevMixDictorPhrases,
    // this.beforeFinishDictorPhrases,
    // this.finishDictorPhrases,
    this.firstDictorPhrasesWithAudioW,
    this.firstBeforeDialogDictorPhrasesWithAudioW,
    this.beforeByOrderMixDictorPhrasesWithAudioW,
    this.insideByOrderMixDictorPhrasesWithAudioW,
    this.beforeBaseWordsDirMixDictorPhrasesWithAudioW,
    this.insideBaseWordsDirMixDictorPhrasesWithAudioW,
    this.beforeBaseWordsRevMixDictorPhrasesWithAudioW,
    this.insideBaseWordsRevMixDictorPhrasesWithAudioW,
    this.beforeAllDirMixDictorPhrasesWithAudioW,
    this.insideAllDirMixDictorPhrasesWithAudioW,
    this.beforeAllRevMixDictorPhrasesWithAudioW,
    this.insideAllRevMixDictorPhrasesWithAudioW,
    this.beforeFinishDictorPhrasesWithAudioW,
    this.finishDictorPhrasesWithAudioW,
  );

  factory MixParam.fromJson(Map<String, dynamic> data) =>
      _$MixParamFromJson(data);

  Map<String, dynamic> toJson() => _$MixParamToJson(this);
}
