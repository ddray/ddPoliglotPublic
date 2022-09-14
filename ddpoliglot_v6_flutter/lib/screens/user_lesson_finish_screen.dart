import 'package:ddpoliglot_v6_flutter/widgets/navigation_drawer_widget.dart';
import 'package:flutter/material.dart';

class UserLessonFinishScreen extends StatefulWidget {
  const UserLessonFinishScreen({Key? key}) : super(key: key);
  static const routeName = '/user_lesson_finish';

  @override
  State<UserLessonFinishScreen> createState() => _UserLessonFinishScreenState();
}

class _UserLessonFinishScreenState extends State<UserLessonFinishScreen> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Урок закончен!!!'),
      ),
      drawer: NavigationDrawer(
        pageType: Pages.finishPage.index,
      ),
    );
  }

  // bool _isInitialized = false;
  // List<DictWord> _words = [];
  // List<DictWord> _wordsFiltered = [];

  // @override
  // void didChangeDependencies() {
  //   final playListProvider =
  //       Provider.of<PlayListProvider>(context, listen: false);

  //   if (!_isInitialized) {
  //     _isInitialized = true;
  //     setState(() {
  //       _words = playListProvider.userLessonData!.userLesson!.userLessonWords
  //           .map((e) {
  //         e.dictWord!.wordLessonType = e.wordType;
  //         e.dictWord!.selected = (e.wordType == 0);
  //         return e.dictWord!;
  //       }).toList();

  //       _wordsFiltered = _words.where((x) => x.grade < 5).toList();
  //     });
  //   }

  //   super.didChangeDependencies();
  // }

  // Future<void> _onChangeGrade(DictWord word, int grade) async {
  //   var oldGrade = word.grade;
  //   if (grade == oldGrade) return;
  //   setState(() {
  //     word.grade = grade.toInt();
  //     word.loading = true;
  //   });

  //   WordUserService.setWordGrade(word.grade, word.wordID, word.wordUserID,
  //           Provider.of<DatabaseProvider>(context, listen: false))
  //       .then((wordUser) {
  //     setState(() {
  //       word.wordUserID = wordUser.wordUserID;
  //       word.loading = false;
  //     });
  //   });
  // }

  // Future<void> _onChangeGradeDismiss(DictWord word, int grade) async {
  //   var oldGrade = word.grade;
  //   if (grade == oldGrade) return;
  //   var wordUser = await WordUserService.setWordGrade(grade, word.wordID,
  //       word.wordUserID, Provider.of<DatabaseProvider>(context, listen: false));

  //   setState(() {
  //     word.grade = grade.toInt();
  //     word.wordUserID = wordUser.wordUserID;
  //   });
  // }

  // @override
  // Widget build(BuildContext context) {
  //   final playListProvider =
  //       Provider.of<PlayListProvider>(context, listen: false);
  //   return Scaffold(
  //       appBar: AppBar(
  //         title: Text(
  //             'Урок ${playListProvider.userLessonData!.userLesson!.num} закончен!!!'),
  //       ),
  //       drawer: NavigationDrawer(
  //         pageType: Pages.finishPage.index,
  //       ),
  //       floatingActionButton: Row(
  //         mainAxisAlignment: MainAxisAlignment.end,
  //         children: [
  //           FloatingActionButton.extended(
  //             heroTag: 'h01',
  //             onPressed: () async {
  //               await playListProvider.goToStart();
  //             },
  //             icon: Transform(
  //               alignment: Alignment.center,
  //               transform: Matrix4.rotationY(math.pi),
  //               child: const Icon(Icons.next_plan_outlined),
  //             ),
  //             label: const Text('Назад'),
  //           ),
  //           const SizedBox(
  //             width: 10,
  //           ),
  //           FloatingActionButton.extended(
  //             heroTag: 'h02',
  //             onPressed: () async {
  //               await playListProvider.clearLesson();
  //             },
  //             icon: const Icon(Icons.next_plan_outlined),
  //             label: const Text('Дальше'),
  //           ),
  //         ],
  //       ),
  //       body: Column(
  //         mainAxisAlignment: MainAxisAlignment.start,
  //         children: [
  //           Expanded(
  //             flex: 1,
  //             child: Container(
  //               margin: const EdgeInsets.symmetric(horizontal: 10),
  //               child: Container(
  //                 alignment: Alignment.center,
  //                 width: double.infinity,
  //                 padding: const EdgeInsets.all(5),
  //                 child: ListView(
  //                   children: const [
  //                     QuestQuestionWidget(
  //                         'Уберите влево слова, которые не нужно больше учить.'),
  //                   ],
  //                 ),
  //               ),
  //             ),
  //           ),
  //           Expanded(
  //               flex: 4,
  //               child: ListView.builder(
  //                   padding: const EdgeInsets.all(8),
  //                   itemCount: _wordsFiltered.length,
  //                   itemBuilder: (BuildContext context, int index) {
  //                     return Dismissible(
  //                       key: Key(_wordsFiltered[index].wordID.toString()),
  //                       onDismissed: (direction) async {
  //                         // var grade =
  //                         //     direction == DismissDirection.endToStart ? 5 : 1;
  //                         await _onChangeGradeDismiss(_wordsFiltered[index], 5);
  //                         setState(() {
  //                           _wordsFiltered.removeAt(index);
  //                         });
  //                       },
  //                       background: Row(
  //                         children: [
  //                           Expanded(
  //                             child: Container(
  //                               color: ColorsUtils.customYellowColor,
  //                               height: double.infinity,
  //                               child: Row(
  //                                 mainAxisAlignment: MainAxisAlignment.start,
  //                                 children: const [
  //                                   SizedBox(
  //                                     width: 10,
  //                                   ),
  //                                   Icon(
  //                                     Icons.star,
  //                                     color: Colors.white,
  //                                     size: 25,
  //                                   ),
  //                                   SizedBox(
  //                                     width: 10,
  //                                   ),
  //                                   Text(
  //                                     'Я знаю это слово.',
  //                                     style: TextStyle(
  //                                         color: Colors.white,
  //                                         fontWeight: FontWeight.bold),
  //                                   ),
  //                                 ],
  //                               ),
  //                             ),
  //                           ),
  //                           Expanded(
  //                             child: Container(
  //                               height: double.infinity,
  //                               color: ColorsUtils.customYellowColor,
  //                               child: Row(
  //                                 mainAxisAlignment: MainAxisAlignment.end,
  //                                 children: [
  //                                   Row(
  //                                     mainAxisAlignment: MainAxisAlignment.end,
  //                                     children: const [
  //                                       Text(
  //                                         'Я знаю это слово.',
  //                                         style: TextStyle(
  //                                             color: Colors.white,
  //                                             fontWeight: FontWeight.bold),
  //                                       ),
  //                                       SizedBox(
  //                                         width: 10,
  //                                       ),
  //                                       Icon(
  //                                         Icons.star,
  //                                         color: Colors.white,
  //                                         size: 25,
  //                                       ),
  //                                       SizedBox(
  //                                         width: 10,
  //                                       ),
  //                                     ],
  //                                   ),
  //                                 ],
  //                               ),
  //                             ),
  //                           ),
  //                         ],
  //                       ),
  //                       child: DictWordItem(
  //                           dictWord: _wordsFiltered[index],
  //                           isLast: index == _wordsFiltered.length - 1,
  //                           canSelected: false,
  //                           canSelectedAll: false,
  //                           withoutCheckbox: true,
  //                           onChangeSelection: null,
  //                           onChangeGrade: (grade) async {
  //                             await _onChangeGrade(
  //                                 _wordsFiltered[index], grade);
  //                           }),
  //                     );
  //                   })),
  //         ],
  //       ));
  // }
}
