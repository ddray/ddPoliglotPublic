// import 'package:ddpoliglot_v6_flutter/services/word_user_service.dart';

// import '../models/data/WordUser.dart';
// import 'package:flutter/material.dart';
// import 'package:provider/provider.dart';
// import 'package:flutter_rating_bar/flutter_rating_bar.dart';

// import '../providers/play_list_data_provider.dart';

// class SelectLessonWordsWidget extends StatelessWidget {
//   const SelectLessonWordsWidget({
//     Key? key,
//   }) : super(key: key);

//   @override
//   Widget build(BuildContext context) {
//     final playListProvider =
//         Provider.of<PlayListProvider>(context, listen: true);

//     // ignore: sized_box_for_whitespace
//     return Container(
//       height: 300,
//       child: ListView.builder(
//         restorationId: 'sampleItemListView',
//         itemCount: playListProvider.words.length,
//         itemBuilder: (BuildContext context, int index) {
//           final item = playListProvider.words[index];

//           return ListTile(
//               title: Text(item.text),
//               subtitle: Text(item.pronunciation ?? '',
//                   style: TextStyle(
//                       fontFamily: 'SourceSansPro',
//                       fontSize: 18,
//                       color: Colors.grey[400])),
//               trailing: RatingBar.builder(
//                 initialRating: item.wordUser?.grade.toDouble() ?? 0,
//                 minRating: 0,
//                 direction: Axis.horizontal,
//                 allowHalfRating: false,
//                 itemCount: 5,
//                 itemPadding: const EdgeInsets.symmetric(horizontal: 0),
//                 itemBuilder: (context, _) => const Icon(
//                   Icons.star,
//                   color: Colors.amber,
//                 ),
//                 onRatingUpdate: (grade) async {
//                   debugPrint(grade.toString());
//                   item.wordUser = await WordUserService.updateWU(item.wordUser ??
//                       WordUser(0, item.wordID, item, playListProvider.userId!,
//                           item.languageID, null, grade.toInt(), 0, 0, 0, 0));
//                 },
//               ),
//               onTap: () {});
//         },
//       ),
//     );
//   }
// }
