import 'dart:math';

import 'package:ddpoliglot_v6_flutter/exeptions/custom_exception.dart';
import 'package:ddpoliglot_v6_flutter/models/aPhWr.dart';
import 'package:ddpoliglot_v6_flutter/models/data/WordPhrase.dart';
import 'package:ddpoliglot_v6_flutter/models/data/WordPhraseTranslation.dart';
import 'package:ddpoliglot_v6_flutter/models/data/WordTranslation.dart';
import 'package:ddpoliglot_v6_flutter/models/play_list.dart';
import 'package:ddpoliglot_v6_flutter/models/play_list_item.dart';
import 'package:flutter/cupertino.dart';
import 'package:http/http.dart' as http;
import 'package:path_provider/path_provider.dart' as syspath;
import 'dart:io' as io;
import 'package:uuid/uuid.dart';

import '../models/data/MixParam.dart';
import '../models/article_by_param_data.dart';
import '../models/data/Word.dart';
import '../models/player_classes.dart';
import 'http_utils.dart';

class PlayListUtils {
  //static phrasesAudioUrl: string = environment.phrasesAudioUrl;

  static Future<List<PlayList>> makePlayLists(
      List<Word> data,
      ArticleByParamData articleByParamData,
      List<Word> wordsForRepetition,
      Function(int, int) setLoading,
      bool isOnLine) async {
    // prepare audio data in source data.
    // all current data is in
    // data: words and phrases
    // articleByParamData: templates to generate articles

    // add audio objects to each word and phrase
    await addPlayListItemsToWordsAndPhrasies(data, setLoading, isOnLine);

    // add audio objects to each repetition word and phrase
    await addPlayListItemsToWordsAndPhrasies(
        wordsForRepetition, setLoading, isOnLine, false);

    // add audio objects to each dictor phrase
    await addPlayListItemsToDictorPhrasies(articleByParamData, isOnLine);

    // prepare list of words and phrases to add for each lesson
    var playListDialogOrWordPhrases = getDialogOrWordPhrases(data);

    // // prepare list of words and phrases to add for each lesson
    // var playListRepeatWordPhrases = getDialogOrWordPhrases(wordsForRepetition);

    // var wordsRepetitionPlayListItems =
    //     wordsForRepetition.map((w) => w.playListItem);
    // var wordPhrasesRepetitionPlayListItems =
    //     wordsForRepetition.map((w) => w.wordPhraseSelected[0].playListItem);

    // generate playlists (articles - trenings)
    List<PlayList> playLists = [];

    // add test playlist
    final testPlayList = generateTestPlayList();
    playLists.add(testPlayList);

    for (var i = 0; i < articleByParamData.mixParamsList.length; i++) {
      // add first dictor phrases (article spicific)
      var mixParam = articleByParamData.mixParamsList[i];
      var playListFirstDictorPhrases = getFirstDictorPhrasesRnd(mixParam);

      // generate playlist based on this mixParam (article)
      var playListByOrderRnd = mixPhrasesByOrderRndMethod(data, mixParam);

      // repeat rnd base words only
      var playListRndBaseWords = mixPhrasesRndMethod(
          data,
          mixParam.active,
          mixParam.repeatBaseWord,
          false,
          false,
          mixParam.beforeBaseWordsDirMixDictorPhrasesWithAudioW,
          mixParam.insideBaseWordsDirMixDictorPhrasesWithAudioW,
          1,
          wordsForRepetition,
          mixParam.repeatRepetitionsWordsInBaseMix);

      // repeat rnd base words only tr first
      var playListRndBaseWordsTrFirst = mixPhrasesRndMethod(
          data,
          mixParam.trActive,
          mixParam.trRepeatBaseWord,
          false,
          true,
          mixParam.beforeBaseWordsRevMixDictorPhrasesWithAudioW,
          mixParam.insideBaseWordsRevMixDictorPhrasesWithAudioW,
          1,
          wordsForRepetition,
          mixParam.repeatRepetitionsWordsInBaseMix);

      // repeat rnd all
      var playListRndAll = mixPhrasesRndMethod(
          data,
          mixParam.active,
          mixParam.repeat,
          true,
          false,
          mixParam.beforeAllDirMixDictorPhrasesWithAudioW,
          mixParam.insideAllDirMixDictorPhrasesWithAudioW,
          2,
          wordsForRepetition,
          mixParam.repeatRepetitionsWordsAndPhrasesInAllMix);

      // repeat rnd all tr first
      var playListRndAllTrFirst = mixPhrasesRndMethod(
          data,
          mixParam.trActive,
          mixParam.trRepeat,
          true,
          true,
          mixParam.beforeAllRevMixDictorPhrasesWithAudioW,
          mixParam.insideAllRevMixDictorPhrasesWithAudioW,
          2,
          wordsForRepetition,
          mixParam.repeatRepetitionsWordsAndPhrasesInAllMix);

      List<PlayListItem> beginning = playListFirstDictorPhrases.isNotEmpty
          ? [...playListFirstDictorPhrases, ...playListDialogOrWordPhrases]
          : [];

      var resultList = mixParam.trFirst
          ? [
              ...beginning,
              ...playListByOrderRnd,
              ...playListRndBaseWordsTrFirst,
              ...playListRndBaseWords,
              ...playListRndAllTrFirst,
              ...playListRndAll
            ]
          : [
              ...beginning,
              ...playListByOrderRnd,
              ...playListRndBaseWords,
              ...playListRndBaseWordsTrFirst,
              ...playListRndAll,
              ...playListRndAllTrFirst
            ];

      var uuid = const Uuid();

      // calculate duration
      var duration = 0;
      for (var playListItem in resultList) {
        duration += (playListItem.speachDuration + playListItem.pause);
      }

      playLists.add(PlayList(uuid.v1().toString(), 'Тренажер: ${i + 1}', true,
          false, resultList, duration, 0));
    }

    // download audio files if need
    List<String> fNames = [];
    for (var playList in playLists) {
      if (playList.name.startsWith('Test')) {
        continue;
      }

      for (var playListItem in playList.items) {
        fNames.add(playListItem.textSpeechFileName);
      }
    }

    // debugPrint('all Names ${fNames.length}');

    final fuNames = fNames.toSet().toList();
    debugPrint('unic Names ${fuNames.length}');

    // // add audio objects
    // debugPrint('start download files');
    // int cntDownloaded = 0;
    // setLoading(80, 0);
    // int index = 0;
    // bool exeption = false;
    // final startTime = DateTime.now();
    // await Future.forEach(fuNames, (String fileName) async {
    //   if (!exeption) {
    //     setLoading(80, (++index * 100) ~/ fuNames.length);
    //     try {
    //       final bool isDownloaded = await requestDownload(fileName, isOnLine);
    //       cntDownloaded += isDownloaded ? 1 : 0;
    //     } catch (e) {
    //       exeption = true;
    //       rethrow;
    //     }
    //   }
    // });

    // var diff = DateTime.now().difference(startTime).inSeconds;
    // debugPrint(
    //     'downloaded $cntDownloaded files from ${fuNames.length}, take $diff seconds');

    await downloadFiles(fuNames, isOnLine, setLoading);
    return playLists;
  }

  static Future<bool> downloadFiles(
      fuNames, isOnLine, Function(int, int) setLoading) async {
    final appDir = await syspath.getApplicationDocumentsDirectory();
    final serverDomen = await HttpUtils.getServerDomen();
    List<String> fileNamesToDownload = [];
    for (var filename in fuNames) {
      final fullFileName = '${appDir.path}/$filename';
      final fileExists = await io.File(fullFileName).exists();
      if (!fileExists) {
        if (!isOnLine) {
          throw CustomException("Try to upload file $filename without internet",
              "Требуется подгрузка данных с сервера, подключите интернет");
        } else {
          fileNamesToDownload.add(filename);
        }
      }
    }

    List<List<String>> fnGroups = [];
    List<String> fnGroup = [];

    for (var fn in fileNamesToDownload) {
      fnGroup.add(fn);
      if (fnGroup.length >= 20) {
        fnGroups.add(fnGroup);
        fnGroup = [];
      }
    }

    if (fnGroup.isNotEmpty) {
      fnGroups.add(fnGroup);
      fnGroup = [];
    }

    setLoading(80, 0);
    int index = 0;
    bool exeption = false;
    final startTime = DateTime.now();
    await Future.forEach(fnGroups, (List<String> filenames) async {
      var requests = filenames.map((filename) {
        try {
          final fileUrl = '$serverDomen/api/Files/GetPhraseAudio/$filename';
          final httpClient = http.Client();
          debugPrint('start download from : $fileUrl');
          final request = http.Request('GET', Uri.parse(fileUrl));
          return httpClient.send(request);
        } catch (e) {
          exeption = true;
          throw CustomException(
              'error download file $filename, ${e.toString()} ',
              "Ошибка: Не удается подгрузить данные для урока с сервера. Сообщение принято, ошибка будет исправлена. Пожалуйста, попробуйте позже.");
        }
      });

      var responses = await Future.wait(requests);

      await Future.forEach(responses, (http.StreamedResponse response) async {
        if (!exeption) {
          final filename =
              (response.request?.url.path ?? '').split('GetPhraseAudio/')[1];
          if (response.statusCode == 200) {
            final fullFileName = '${appDir.path}/$filename';
            var bytes = await response.stream.toBytes();
            var file = io.File(fullFileName);
            await file.writeAsBytes(bytes);
            debugPrint('end save download from : $filename');
            setLoading(80, (++index * 100) ~/ fileNamesToDownload.length);
            return true;
          } else {
            throw CustomException(
                'error download file $filename, ${response.statusCode} ',
                "Ошибка: Не удается подгрузить данные для урока с сервера. Сообщение принято, ошибка будет исправлена. Пожалуйста, попробуйте позже.");
          }
        }
      });
    });

    var diff = DateTime.now().difference(startTime).inSeconds;
    debugPrint(
        'need download ${fileNamesToDownload.length} files from ${fuNames.length}, take $diff seconds');
    return true;
  }

  static List<PlayListItem> getFirstDictorPhrasesRnd(MixParam mixParam) {
    List<PlayListItem> playListFirstDictorPhrases = [];
    var phraseBlock =
        getRndDictorPhraseBlock(mixParam.firstDictorPhrasesWithAudioW);

    for (var phrase in phraseBlock) {
      if (phrase.playListItem != null) {
        var playListItem = PlayListItem.fromOther(phrase.playListItem!);
        playListItem.activityType = PlayListActivityTypes.textFirst.index;
        playListFirstDictorPhrases.add(playListItem);
      }
    }

    return playListFirstDictorPhrases;
  }

  static List<PlayListItem> getDialogOrWordPhrases(List<Word> data) {
    List<PlayListItem> list = [];
    for (var word in data) {
      var playListItem = PlayListItem.fromOther(word.playListItem!);
      playListItem.activityType = PlayListActivityTypes.textFirst.index;
      playListItem.pause = 1;
      list.add(playListItem);

      for (var wordPhrase in word.wordPhraseSelected!) {
        var playListItem1 = PlayListItem.fromOther(wordPhrase.playListItem!);
        playListItem1.activityType = PlayListActivityTypes.textFirst.index;
        playListItem1.pause = 1;
        list.add(playListItem1);
      }
    }

    return list;
  }

  static List<PlayListItem> mixPhrasesRndMethod(
      List<Word> data,
      bool active,
      int repeat,
      bool includePhrases,
      bool trFirst,
      List<List<List<APhWr>>> before,
      List<List<List<APhWr>>> inside,
      int typeOfSourceForMix,
      List<Word> wordsForRepetition,
      int repeatRepeatQty) {
    List<PlayListItem> resultList = [];
    List<PlailablePare> list = [];
    List<int> rndList = [];

    if (!active) {
      return resultList;
    }

    // prepare plain list of phrases
    List<PlailablePare> phPars = [];
    for (var word in data) {
      phPars
          .add(PlailablePare(source: word, translation: word.wordTranslation));
      if (includePhrases) {
        for (var wordPhrase in word.wordPhraseSelected!) {
          phPars.add(PlailablePare(
              source: wordPhrase,
              translation: wordPhrase.wordPhraseTranslation));
        }
      }
    }

    for (var r = 0; r < repeat; r++) {
      for (var i = 0; i < phPars.length; i++) {
        list.add(phPars[i]);
        rndList.add(list.length - 1);
      }
    }

    List<PlailablePare> phParsRep = [];
    for (var word in wordsForRepetition) {
      phParsRep
          .add(PlailablePare(source: word, translation: word.wordTranslation));
      if (includePhrases) {
        for (var wordPhrase in word.wordPhraseSelected!) {
          phParsRep.add(PlailablePare(
              source: wordPhrase,
              translation: wordPhrase.wordPhraseTranslation));
        }
      }
    }

    for (var r = 0; r < repeatRepeatQty; r++) {
      for (var i = 0; i < phParsRep.length; i++) {
        list.add(phParsRep[i]);
        rndList.add(list.length - 1);
      }
    }

    // rnd mix
    if (repeat > 0) {
      var len = (phPars.length * repeat) + (phParsRep.length * repeatRepeatQty);
      var cnt = len - 1;
      var mem = 0;
      for (var r = 0; r < len; r++) {
        var rndNum = 0;
        var rndTry = 0;
        do {
          // difrent from prev
          rndNum = getRandomInt(0, cnt - r);
          rndTry++;
        } while (list[rndList[rndNum]].source!.playListItem!.itemID == mem &&
            rndTry < 3);
        mem = list[rndList[rndNum]].source!.playListItem!.itemID;

        var ind = rndList[rndNum];
        if (trFirst) {
          var playListItem =
              PlayListItem.fromOther(list[ind].translation!.playListItem!);
          playListItem.activityType =
              PlayListActivityTypes.translationFirst.index;
          playListItem.childrenType = ChildrenType.rndTr.index;
          resultList.add(playListItem);

          playListItem =
              PlayListItem.fromOther(list[ind].source!.playListItem!);
          playListItem.activityType =
              PlayListActivityTypes.translationFirst.index;
          playListItem.childrenType = ChildrenType.rnd.index;
          resultList.add(playListItem);
        } else {
          var playListItem =
              PlayListItem.fromOther(list[ind].source!.playListItem!);
          playListItem.activityType = PlayListActivityTypes.textFirst.index;
          playListItem.childrenType = ChildrenType.rnd.index;
          resultList.add(playListItem);

          playListItem =
              PlayListItem.fromOther(list[ind].translation!.playListItem!);
          playListItem.activityType = PlayListActivityTypes.textFirst.index;
          playListItem.childrenType = ChildrenType.rndTr.index;
          resultList.add(playListItem);
        }

        rndList = rndList.where((x) => x != ind).toList();
      }

      resultList = addDictorPhrasesAndRecalcPauses(
          resultList, before, inside, typeOfSourceForMix, trFirst);
    }

    return resultList;
  }

  static int getRandomInt(int min, int max) {
    if (min == max) return min;
    var rn = Random();
    return min + rn.nextInt(max - min);
  }

  static List<PlayListItem> mixPhrasesByOrderRndMethod(
      List<Word> data, MixParam mixParam) {
    List<PlayListItem> resultList = [];
    if (!mixParam.active) {
      return resultList;
    }

    // add words and phrases to list
    for (var word in data) {
      // add word
      fillByOrder(word, word.wordTranslation!, mixParam, resultList);

      // add word's phrases
      for (var wordPhrase in word.wordPhraseSelected!) {
        fillByOrder(wordPhrase, wordPhrase.wordPhraseTranslation!, mixParam,
            resultList);
      }
    }

    if (resultList.isNotEmpty) {
      resultList = addDictorPhrasesAndRecalcPauses(
          resultList,
          mixParam.beforeByOrderMixDictorPhrasesWithAudioW,
          mixParam.insideByOrderMixDictorPhrasesWithAudioW,
          0,
          false);
    }

    return resultList;
  }

  static List<PlayListItem> addDictorPhrasesAndRecalcPauses(
      List<PlayListItem> list,
      List<List<List<APhWr>>> before,
      List<List<List<APhWr>>> inside,
      int typeOfSourceForMix,
      bool trFirst) {
    List<PlayListItem> resultList = [];

    if (list.isEmpty) {
      return [];
    }

    final beforeRndDictorPhraseBlock = getRndDictorPhraseBlock(before);

    for (var phrase in beforeRndDictorPhraseBlock) {
      if (phrase.playListItem != null) {
        var playListItem = PlayListItem.fromOther(phrase.playListItem!);
        playListItem.childrenType = ChildrenType.no.index;
        playListItem.activityType = PlayListActivityTypes.textFirst.index;
        resultList.add(playListItem);
      }
    }

    var needStepInsert = typeOfSourceForMix == 0
        ? 12 * 2 // first Order mix
        : typeOfSourceForMix == 1
            ? 20 * 2 // mix Base words
            : 14 * 2; // mix all
    var countToInsert = (list.length / needStepInsert) - 1;

    if (countToInsert < 1) {
      needStepInsert = list.length ~/ 2;
      countToInsert = 1;
    }

    final insideRndDictorPhraseBlocks =
        getInsideRndDictorPhraseBlocks(inside, countToInsert);

    var needInsertCnt = needStepInsert;
    var currIndexToInsert = 0;

    for (var item in list) {
      if (insideRndDictorPhraseBlocks.isNotEmpty) {
        if (needInsertCnt < 0 &&
            ((typeOfSourceForMix == 0 &&
                    item.childrenType == ChildrenType.orderRpt.index) ||
                ((typeOfSourceForMix != 0 && trFirst == false) &&
                    (item.childrenType == ChildrenType.rnd.index ||
                        item.childrenType == ChildrenType.rndRepeatWrd.index ||
                        item.childrenType ==
                            ChildrenType.rndRepeatPhrases.index)) ||
                ((typeOfSourceForMix != 0 && trFirst == true) &&
                    (item.childrenType == ChildrenType.rndTr.index ||
                        item.childrenType ==
                            ChildrenType.rndRepeatWrdTr.index ||
                        item.childrenType ==
                            ChildrenType.rndRepeatPhrasesTr.index)))) {
          // only before this type of items

          for (var phrase in insideRndDictorPhraseBlocks[currIndexToInsert]) {
            if (phrase.playListItem != null) {
              var playListItem = PlayListItem.fromOther(phrase.playListItem!);
              playListItem.activityType = PlayListActivityTypes.textFirst.index;
              playListItem.childrenType = ChildrenType.no.index;
              resultList.add(playListItem);
            }
          }
          currIndexToInsert++;
          if (currIndexToInsert >= insideRndDictorPhraseBlocks.length) {
            currIndexToInsert = 0;
          }

          needInsertCnt = needStepInsert;
        } else {
          needInsertCnt--;
        }
      }

      var newItem = PlayListItem.fromOther(item);
      if ((typeOfSourceForMix == 0 &&
              (newItem.itemType ==
                      PlayListItemTypes.wordPhraseTranslation.index ||
                  newItem.itemType ==
                      PlayListItemTypes.wordTranslation.index)) ||
          ((newItem.itemType == PlayListItemTypes.dialogOrWordPhrase.index &&
              newItem.itemType == PlayListItemTypes.dictorPhrase.index))) {
        // after translation in by order mix or after dictor phrases
        item.pause = 2;
      } else if (typeOfSourceForMix != 0 &&
          (newItem.itemType == PlayListItemTypes.wordPhraseTranslation.index ||
              newItem.itemType == PlayListItemTypes.wordTranslation.index)) {
        newItem.pause = !trFirst
            ? (newItem.childrenType == ChildrenType.rnd.index ||
                    newItem.childrenType ==
                        ChildrenType.rndRepeatPhrases.index ||
                    newItem.childrenType == ChildrenType.rndRepeatWrd.index)
                ? newItem.srcWordsQty + 1 // children repeat
                : newItem.srcWordsQty
            : newItem.srcWordsQty + 1; // after dictor
      } else {
        newItem.pause = calcPauseByWordsQty(newItem.srcWordsQty);
      }

      resultList.add(newItem);
    }

    return resultList;
  }

  static Future<void> addPlayListItemsToDictorPhrasies(
      ArticleByParamData articleByParamData, bool isOnLine) async {
    // go throw all params and make playListItems for each dictor phrase
    int cnt = 0;
    await Future.forEach(articleByParamData.mixParamsList,
        (MixParam mixParam) async {
      cnt += await addPlayListItemsToDictorPhrase(
          mixParam.firstDictorPhrasesWithAudioW, isOnLine);

      cnt += await addPlayListItemsToDictorPhrase(
          mixParam.beforeByOrderMixDictorPhrasesWithAudioW, isOnLine);

      cnt += await addPlayListItemsToDictorPhrase(
          mixParam.insideByOrderMixDictorPhrasesWithAudioW, isOnLine);

      cnt += await addPlayListItemsToDictorPhrase(
          mixParam.beforeBaseWordsDirMixDictorPhrasesWithAudioW, isOnLine);

      cnt += await addPlayListItemsToDictorPhrase(
          mixParam.insideBaseWordsDirMixDictorPhrasesWithAudioW, isOnLine);

      cnt += await addPlayListItemsToDictorPhrase(
          mixParam.beforeBaseWordsRevMixDictorPhrasesWithAudioW, isOnLine);

      cnt += await addPlayListItemsToDictorPhrase(
          mixParam.insideBaseWordsRevMixDictorPhrasesWithAudioW, isOnLine);

      cnt += await addPlayListItemsToDictorPhrase(
          mixParam.beforeAllDirMixDictorPhrasesWithAudioW, isOnLine);

      cnt += await addPlayListItemsToDictorPhrase(
          mixParam.insideAllDirMixDictorPhrasesWithAudioW, isOnLine);

      cnt += await addPlayListItemsToDictorPhrase(
          mixParam.beforeAllRevMixDictorPhrasesWithAudioW, isOnLine);

      cnt += await addPlayListItemsToDictorPhrase(
          mixParam.insideAllRevMixDictorPhrasesWithAudioW, isOnLine);

      cnt += await addPlayListItemsToDictorPhrase(
          mixParam.beforeFinishDictorPhrasesWithAudioW, isOnLine);

      cnt += await addPlayListItemsToDictorPhrase(
          mixParam.finishDictorPhrasesWithAudioW, isOnLine);
    });

    debugPrint('!!!!!!!!!!!!! ---------- dictor phases: $cnt');
  }

  static Future<int> addPlayListItemsToDictorPhrase(
      List<List<List<APhWr>>>? dictorPhrasesVariants, bool isOnLine) async {
    int cnt = 0;
    if (dictorPhrasesVariants != null) {
      await Future.forEach(dictorPhrasesVariants,
          (List<List<APhWr>> dictorPhrasesBlocks) async {
        await Future.forEach(dictorPhrasesBlocks,
            (List<APhWr> blockPhrases) async {
          await Future.forEach(blockPhrases, (APhWr phrase) async {
            if ((phrase.t!).length > 2) {
              var newItem = PlayListItem(
                itemID: 0,
                itemParentID: 0,
                itemRef: null,
                itemParentRef: null,
                text: phrase.t!,
                pronunciation: '',
                textSpeechFileName: phrase.tfName ?? "",
                speachDuration: phrase.sd!,
                pause: 1,
                itemType: PlayListItemTypes.dictorPhrase.index,
              );

              // await requestDownload(newItem.textSpeechFileName, isOnLine);
              phrase.playListItem = newItem;
              cnt++;
            }
          });
        });
      });
    }

    return cnt;
  }

  static void fillByOrder(Plailable item, Plailable itemTranslation,
      MixParam mixParams, List<PlayListItem> list) {
    for (var index = 0; index < mixParams.repeatOrder; index++) {
      var playListItem = PlayListItem.fromOther(item.playListItem!);
      playListItem.order = list.length;
      playListItem.activityType = PlayListActivityTypes.textFirst.index;
      playListItem.childrenType = ChildrenType.orderRpt.index;
      list.add(playListItem);

      playListItem = PlayListItem.fromOther(itemTranslation.playListItem!);
      playListItem.order = list.length;
      playListItem.activityType = PlayListActivityTypes.textFirst.index;
      playListItem.childrenType = ChildrenType.orderRptTr.index;
      list.add(playListItem);

      if (mixParams.addSlow2InRepeatOrder) {
        playListItem = PlayListItem.fromOther(item.playListItemSpeed2!);
        playListItem.order = list.length;
        playListItem.activityType = PlayListActivityTypes.textFirst.index;
        playListItem.childrenType = ChildrenType.orderRpt2.index;
        list.add(playListItem);
      }

      if (mixParams.addSlowInRepeatOrder) {
        playListItem = PlayListItem.fromOther(item.playListItemSpeed1!);
        playListItem.order = list.length;
        playListItem.activityType = PlayListActivityTypes.textFirst.index;
        playListItem.childrenType = ChildrenType.orderRpt3.index;
        list.add(playListItem);
      }

      if (mixParams.addSlow2InRepeatOrder || mixParams.addSlowInRepeatOrder) {
        playListItem = PlayListItem.fromOther(itemTranslation.playListItem!);
        playListItem.order = list.length;
        playListItem.activityType = PlayListActivityTypes.textFirst.index;
        playListItem.childrenType = ChildrenType.orderRptTr.index;
        list.add(playListItem);
      }
    }
  }

  static Future<void> addPlayListItemsToWordsAndPhrasies(
      List<Word> data, Function(int, int) setLoading, bool isOnLine,
      [bool isMainWord = true]) async {
    List<PlayListItem> list = [];
    for (var word in data) {
      word.playListItem = PlayListItem(
        itemID: word.wordID,
        itemParentID: 0,
        itemRef: word,
        itemParentRef: null,
        text: word.text,
        pronunciation: word.pronunciation ?? '',
        textSpeechFileName: word.textSpeechFileName ?? "",
        speachDuration: word.speachDuration,
        pause: word.speachDuration,
        itemType: PlayListItemTypes.word.index,
        srcWordsQty: word.text.split(" ").length,
      );

      list.add(word.playListItem!);
      if (isMainWord) {
        word.playListItemSpeed1 = PlayListItem(
          itemID: word.wordID,
          itemParentID: 0,
          itemRef: word,
          itemParentRef: null,
          text: word.text,
          pronunciation: word.pronunciation ?? '',
          textSpeechFileName: word.textSpeechFileNameSpeed1 ?? "",
          speachDuration: word.speachDurationSpeed1,
          pause: word.speachDurationSpeed1,
          itemType: PlayListItemTypes.wordSpeed1.index,
          srcWordsQty: word.text.split(" ").length,
        );

        list.add(word.playListItemSpeed1!);

        word.playListItemSpeed2 = PlayListItem(
          itemID: word.wordID,
          itemParentID: 0,
          itemRef: word,
          itemParentRef: null,
          text: word.text,
          pronunciation: word.pronunciation ?? '',
          textSpeechFileName: word.textSpeechFileNameSpeed2 ?? "",
          speachDuration: word.speachDurationSpeed2,
          pause: word.speachDurationSpeed2,
          itemType: PlayListItemTypes.wordSpeed2.index,
          srcWordsQty: word.text.split(" ").length,
        );
        list.add(word.playListItemSpeed2!);
      }

      word.wordTranslation?.playListItem = PlayListItem(
        itemID: word.wordTranslation?.wordTranslationID ?? 0,
        itemParentID: word.wordID,
        itemRef: word.wordTranslation,
        itemParentRef: word,
        text: word.wordTranslation?.text ?? "",
        pronunciation: '',
        textSpeechFileName: word.wordTranslation?.textSpeechFileName ?? "",
        speachDuration: word.wordTranslation?.speachDuration ?? 0,
        pause: word.wordTranslation?.speachDuration ?? 0,
        itemType: PlayListItemTypes.wordTranslation.index,
        srcWordsQty: word.text.split(" ").length,
      );
      list.add(word.wordTranslation!.playListItem!);

      for (var wordPhrase in word.wordPhraseSelected!) {
        wordPhrase.playListItem = PlayListItem(
          itemID: wordPhrase.wordPhraseID,
          itemParentID: word.wordID,
          itemRef: wordPhrase,
          itemParentRef: word,
          text: wordPhrase.text,
          pronunciation: '',
          textSpeechFileName: wordPhrase.textSpeechFileName ?? "",
          speachDuration: wordPhrase.speachDuration,
          pause: wordPhrase.speachDuration,
          itemType: PlayListItemTypes.wordPhrase.index,
          srcWordsQty: wordPhrase.text.split(" ").length,
        );
        list.add(wordPhrase.playListItem!);

        if (isMainWord) {
          // add wordPhrase text Speed1
          wordPhrase.playListItemSpeed1 = PlayListItem(
            itemID: wordPhrase.wordPhraseID,
            itemParentID: word.wordID,
            itemRef: wordPhrase,
            itemParentRef: word,
            text: wordPhrase.text,
            pronunciation: '',
            textSpeechFileName: wordPhrase.textSpeechFileNameSpeed1 ?? "",
            speachDuration: wordPhrase.speachDurationSpeed1,
            pause: wordPhrase.speachDurationSpeed1,
            itemType: PlayListItemTypes.wordPhraseSpeed1.index,
            srcWordsQty: wordPhrase.text.split(" ").length,
          );
          list.add(wordPhrase.playListItemSpeed1!);

          wordPhrase.playListItemSpeed2 = PlayListItem(
            itemID: wordPhrase.wordPhraseID,
            itemParentID: word.wordID,
            itemRef: wordPhrase,
            itemParentRef: word,
            text: wordPhrase.text,
            pronunciation: '',
            textSpeechFileName: wordPhrase.textSpeechFileNameSpeed2 ?? "",
            speachDuration: wordPhrase.speachDurationSpeed2,
            pause: wordPhrase.speachDurationSpeed2,
            itemType: PlayListItemTypes.wordPhraseSpeed2.index,
            srcWordsQty: wordPhrase.text.split(" ").length,
          );
          list.add(wordPhrase.playListItemSpeed2!);
        }

        wordPhrase.wordPhraseTranslation!.playListItem = PlayListItem(
          itemID: wordPhrase.wordPhraseTranslation!.wordPhraseTranslationID,
          itemParentID: wordPhrase.wordPhraseID,
          itemRef: wordPhrase.wordPhraseTranslation,
          itemParentRef: wordPhrase,
          text: wordPhrase.wordPhraseTranslation!.text,
          pronunciation: '',
          textSpeechFileName:
              wordPhrase.wordPhraseTranslation!.textSpeechFileName ?? "",
          speachDuration: wordPhrase.wordPhraseTranslation!.speachDuration,
          pause: wordPhrase.wordPhraseTranslation!.speachDuration,
          itemType: PlayListItemTypes.wordPhraseTranslation.index,
          srcWordsQty: wordPhrase.text.split(" ").length,
        );

        list.add(wordPhrase.wordPhraseTranslation!.playListItem!);
      }
    }

    // // add audio objects
    // debugPrint('start download files for main: $isMainWord');
    // int cntDownloaded = 0;
    // setLoading(80, 0);
    // int index = 0;
    // await Future.forEach(list, (PlayListItem item) async {
    //   setLoading(isMainWord ? 80 : 90, (++index * 100) ~/ list.length);
    //   final bool isDownloaded =
    //       await requestDownload(item.textSpeechFileName, isOnLine);
    //   cntDownloaded += isDownloaded ? 1 : 0;
    // });
    // debugPrint(
    //     'downloaded $cntDownloaded files from ${list.length}, main: $isMainWord');
  }

  // static Future<bool> requestDownload(String filename, bool isOnLine) async {
  //   final appDir = await syspath.getApplicationDocumentsDirectory();
  //   final fullFileName = '${appDir.path}/$filename';
  //   final serverDomen = await HttpUtils.getServerDomen();
  //   final fileUrl = '$serverDomen/api/Files/GetPhraseAudio/$filename';

  //   final fileExists = await io.File(fullFileName).exists();
  //   if (!fileExists) {
  //     if (!isOnLine) {
  //       throw Exception("--------------     No Internet        ----------");
  //     }

  //     var httpClient = http.Client();
  //     var request = http.Request('GET', Uri.parse(fileUrl));
  //     debugPrint('start download from : $fileUrl');
  //     var response = await httpClient.send(request);
  //     if (response.statusCode == 200) {
  //       var bytes = await response.stream.toBytes();
  //       var file = io.File(fullFileName);
  //       await file.writeAsBytes(bytes);
  //       debugPrint('end download from : $fileUrl');
  //       return true;
  //     }
  //   }

  //   return false;
  // }

  static List<APhWr> getRndDictorPhraseBlock(List<List<List<APhWr>>> before) {
    if (before.isEmpty) {
      return [];
    }

    var variantIndex = getRandomInt(0, before.length - 1);
    var blockIndex = getRandomInt(0, before[variantIndex].length - 1);
    return before[variantIndex][blockIndex];
  }

  static List<List<APhWr>> getInsideRndDictorPhraseBlocks(
      List<List<List<APhWr>>> inside, double countToInsert) {
    if (inside.isEmpty || countToInsert <= 0) {
      return [];
    }

    List<List<APhWr>> result = [];
    var inxVariant = 0;
    var cntAdded = 0;
    while (cntAdded < countToInsert) {
      var block = inside[inxVariant];
      var phrase = block[getRandomInt(0, block.length - 1)];
      result.add(phrase);
      cntAdded++;
      if (++inxVariant >= inside.length) {
        inxVariant = 0;
      }
    }

    return result;
  }

  static int calcPauseByWordsQty(int qty) {
    return qty < 2
        ? qty + 1
        : qty < 3
            ? qty
            : qty < 5
                ? qty - 1
                : 5;
  }

  static String seconds2String(int seconds) {
    var duration = Duration(seconds: seconds);
    String twoDigits(int n) => n.toString().padLeft(2, "0");
    String twoDigitMinutes = twoDigits(duration.inMinutes.remainder(60));
    String twoDigitSeconds = twoDigits(duration.inSeconds.remainder(60));
    return "${twoDigits(duration.inHours)}:$twoDigitMinutes:$twoDigitSeconds";
  }

  static PlayList generateTestPlayList() {
    List<PlayListItem> list = [];

    addTestWordItems(
        list: list,
        wordText: 'Short',
        wordPron: 'p-short',
        wordTr: 'короткий',
        dir: 0);
    addTestWordItems(
        list: list,
        wordText: 'Middle-Word',
        wordPron: 'p-middle-word',
        wordTr: 'Среднее слово',
        dir: 0);
    addTestWordItems(
        list: list,
        wordText: 'Long-Long-Word, Long-Long-Word, Long-Long-Word',
        wordPron: 'plonglongword, plonglongword, plonglongword',
        wordTr: 'ДлинноеСлово, ДлинноеСлово, ДлинноеСлово',
        dir: 0);

    addTestWordItems(
        list: list,
        wordText: 'Short',
        wordPron: 'p-short',
        wordTr: 'короткий',
        dir: 1);
    addTestWordItems(
        list: list,
        wordText: 'Middle-Word',
        wordPron: 'p-middle-word',
        wordTr: 'Среднее слово',
        dir: 1);
    addTestWordItems(
        list: list,
        wordText: 'Long-Long-Word, Long-Long-Word, Long-Long-Word',
        wordPron: 'plonglongword, plonglongword, plonglongword',
        wordTr: 'ДлинноеСлово, ДлинноеСлово, ДлинноеСлово',
        dir: 1);

    addTestPhraseItems(
        list: list, text: 'Short Phrase text', tr: 'Короткая фраза', dir: 0);
    addTestPhraseItems(
        list: list,
        text: 'Middle Phrase text Middle Phrase text',
        tr: 'Средняя фраза Средняя фраза Средняя фраза',
        dir: 0);
    addTestPhraseItems(
        list: list,
        text:
            'Long Phrase text Long Phrase text Long Phrase text Long Phrase text',
        tr: 'Длинная фраза Длинная фраза Длинная фраза Длинная фраза',
        dir: 0);

    addTestPhraseItems(
        list: list, text: 'Short Phrase text', tr: 'Короткая фраза', dir: 1);
    addTestPhraseItems(
        list: list,
        text: 'Middle Phrase text Middle Phrase text',
        tr: 'Средняя фраза Средняя фраза Средняя фраза',
        dir: 1);
    addTestPhraseItems(
        list: list,
        text:
            'Long Phrase text Long Phrase text Long Phrase text Long Phrase text',
        tr: 'Длинная фраза Длинная фраза Длинная фраза Длинная фраза',
        dir: 1);

    PlayListItem item;
    // DictorTextFirst = 8,
    item = PlayListItem(
        itemType: PlayListItemTypes.dictorPhrase.index,
        activityType: PlayListActivityTypes.textFirst.index,
        text: 'Dictor text',
        pronunciation: '',
        textSpeechFileName: '1586874283_1_phrase.mp3',
        itemRef: null);

    list.add(item);
    var uuid = const Uuid();
    final playList =
        PlayList(uuid.v1().toString(), 'Test: 0', true, false, list, 10, 0);
    return playList;
  }

  static Word createWord(
      {required String wordText,
      required String wordPron,
      required String wordTr}) {
    final word = Word(
        0,
        0,
        null,
        wordText,
        0,
        wordPron,
        0,
        '1586874283_1_phrase.mp3',
        2,
        0,
        '1586874283_1_phrase.mp3',
        2,
        0,
        '1586874283_1_phrase.mp3',
        2,
        [],
        WordTranslation(
            0, 0, null, 0, null, wordTr, 0, '1586874283_1_phrase.mp3', 2, 0),
        null,
        null,
        0);
    return word;
  }

  static WordPhrase createWordPhrase(
      {required String text, required String tr}) {
    final wordPhrase = WordPhrase(
      0,
      0,
      null,
      text,
      0,
      '1586874283_1_phrase.mp3',
      2,
      0,
      '1586874283_1_phrase.mp3',
      2,
      0,
      '1586874283_1_phrase.mp3',
      2,
      0,
      null,
      [],
      WordPhraseTranslation(
        0,
        0,
        null,
        0,
        null,
        tr,
        0,
        '1586874283_1_phrase.mp3',
        0,
      ),
      0,
      0,
    );

    return wordPhrase;
  }

  static void addTestWordItems(
      {required List<PlayListItem> list,
      required String wordText,
      required String wordPron,
      required String wordTr,
      required int dir}) {
    PlayListItem item;
    final word =
        createWord(wordText: wordText, wordPron: wordPron, wordTr: wordTr);

    if (dir == 0 || dir == 3) {
      // TextYesPronYesTranslNo_TextFirst = 0,
      item = PlayListItem(
          itemType: PlayListItemTypes.word.index,
          activityType: PlayListActivityTypes.textFirst.index,
          text: word.text,
          pronunciation: word.pronunciation!,
          textSpeechFileName: word.textSpeechFileName!);
      list.add(item);

      // TextYesPronYesTranslYesTextFirst = 2,
      item = PlayListItem(
          itemType: PlayListItemTypes.wordTranslation.index,
          activityType: PlayListActivityTypes.textFirst.index,
          text: word.wordTranslation!.text,
          pronunciation: word.pronunciation!,
          textSpeechFileName: word.textSpeechFileName!,
          itemParentRef: word);
      list.add(item);
    }

    if (dir == 1 || dir == 3) {
      // TextNoPronNoTranslYesTranslFirst = 3,
      item = PlayListItem(
          itemType: PlayListItemTypes.wordTranslation.index,
          activityType: PlayListActivityTypes.translationFirst.index,
          text: word.wordTranslation!.text,
          pronunciation: '',
          textSpeechFileName: word.textSpeechFileName!,
          itemRef: word);
      list.add(item);

      // TextYesPronYesTranslYesTranslFirst = 1,
      item = PlayListItem(
          itemType: PlayListItemTypes.word.index,
          activityType: PlayListActivityTypes.translationFirst.index,
          text: word.text,
          pronunciation: word.pronunciation!,
          textSpeechFileName: word.textSpeechFileName!,
          itemRef: word);
      list.add(item);
    }
  }

  static void addTestPhraseItems(
      {required List<PlayListItem> list,
      required String text,
      required String tr,
      required int dir}) {
    WordPhrase wordPhrase;
    PlayListItem item;
    wordPhrase = createWordPhrase(text: text, tr: tr);

    if (dir == 0 || dir == 3) {
      // TextYesTranslNoTextFirst = 4,
      item = PlayListItem(
          itemType: PlayListItemTypes.wordPhrase.index,
          activityType: PlayListActivityTypes.textFirst.index,
          text: wordPhrase.text,
          pronunciation: '',
          textSpeechFileName: wordPhrase.textSpeechFileName!,
          itemRef: null);
      list.add(item);

      // TextYesTranslYesTextFirst = 6,
      item = PlayListItem(
          itemType: PlayListItemTypes.wordPhraseTranslation.index,
          activityType: PlayListActivityTypes.textFirst.index,
          text: wordPhrase.wordPhraseTranslation!.text,
          pronunciation: '',
          textSpeechFileName: wordPhrase.textSpeechFileName!,
          itemParentRef: wordPhrase);
      list.add(item);
    }

    if (dir == 1 || dir == 3) {
      // TextNoTranslYesTranslFirst = 7,
      item = PlayListItem(
          itemType: PlayListItemTypes.wordPhraseTranslation.index,
          activityType: PlayListActivityTypes.translationFirst.index,
          text: wordPhrase.wordPhraseTranslation!.text,
          pronunciation: '',
          textSpeechFileName: wordPhrase.textSpeechFileName!,
          itemParentRef: wordPhrase);
      list.add(item);

      // TextYesTranslYesTranslFirst = 5,
      item = PlayListItem(
          itemType: PlayListItemTypes.wordPhrase.index,
          activityType: PlayListActivityTypes.translationFirst.index,
          text: wordPhrase.text,
          pronunciation: '',
          textSpeechFileName: wordPhrase.textSpeechFileName!,
          itemRef: wordPhrase);
      list.add(item);
    }
  }
}
