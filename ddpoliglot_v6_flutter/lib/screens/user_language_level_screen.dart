// import 'package:ddpoliglot_v6_flutter/models/data/user_language_level.dart';
// import 'package:ddpoliglot_v6_flutter/providers/user_settings_provider.dart';
// import 'package:ddpoliglot_v6_flutter/services/user_language_level_service.dart';
// import 'package:flutter/material.dart';
// import 'package:provider/provider.dart';

// class UserLanguageLevelScreen extends StatefulWidget {
//   const UserLanguageLevelScreen({Key? key}) : super(key: key);
//   static const routeName = '/user_language_level';

//   @override
//   _UserLanguageLevelScreenState createState() =>
//       _UserLanguageLevelScreenState();
// }

// class _UserLanguageLevelScreenState extends State<UserLanguageLevelScreen> {
//   late Future<UserLanguageLevel?> _userLanguageLevel;
//   var _autovalidateMode = AutovalidateMode.disabled;
//   int? _result;
//   bool _loading = false;

//   @override
//   initState() {
//     super.initState();
//     _userLanguageLevel = UserLanguageLevelService.get();
//   }

//   bool validate() {
//     if (_result != null) {
//       return true;
//     }

//     return false;
//   }

//   Future<void> save(BuildContext context, int? args) async {
//     if (validate()) {
//       setState(() {
//         _loading = true;
//       });

//       await UserLanguageLevelService.setByLevel(_result!);
//       final userSettingsProvider =
//           Provider.of<UserSettingsProvider>(context, listen: false);
//       final appSettings = userSettingsProvider.appSettings;
//       final userLanguageLevel = UserLanguageLevel(
//           0, appSettings!.learnLanguage!.languageID, '', _result!);
//       userSettingsProvider
//           .save(appSettings.copyWith(userLanguageLevel: userLanguageLevel));
//       if ((args ?? 0) == 1) {
//         Navigator.of(context).pop();
//       }
//     } else {
//       setState(() {
//         _autovalidateMode = AutovalidateMode.always;
//       });
//     }
//   }

//   @override
//   Widget build(BuildContext context) {
//     debugPrint('!!!! UserLanguageLevelScreen');
//     final args = ModalRoute.of(context)!.settings.arguments as int?;
//     return Scaffold(
//       appBar: AppBar(
//         title: const Text('User Language level'),
//         actions: [
//           IconButton(
//             icon: const Icon(Icons.save),
//             onPressed: _loading
//                 ? null
//                 : () async {
//                     await save(context, args);
//                   },
//           ),
//         ],
//       ),
//       floatingActionButton: FloatingActionButton.extended(
//         onPressed: _loading ? null : () async => await save(context, args),
//         icon: const Icon(Icons.save),
//         label: const Text('Save'),
//       ),
//       body: Container(
//           padding: const EdgeInsets.symmetric(horizontal: 10.0),
//           child: FutureBuilder<UserLanguageLevel?>(
//             future: _userLanguageLevel,
//             builder: (
//               BuildContext context,
//               AsyncSnapshot<UserLanguageLevel?> snapshot,
//             ) {
//               if (snapshot.hasError) {
//                 throw Exception(
//                     snapshot.error.toString() + snapshot.stackTrace.toString());
//               } else if (snapshot.connectionState != ConnectionState.waiting) {
//                 if (_result == null && snapshot.data?.level != null) {
//                   _result = snapshot.data?.level;
//                   debugPrint('start level is : $_result');
//                 }

//                 return _loading
//                     ? const Center(child: CircularProgressIndicator())
//                     : ListView(
//                         children: [
//                           if (_autovalidateMode == AutovalidateMode.always &&
//                               !validate())
//                             Container(
//                               margin: const EdgeInsets.all(10),
//                               child: const Text(
//                                 'Please select you level!!',
//                                 style: TextStyle(color: Colors.red),
//                               ),
//                             ),
//                           RadioListTile<int>(
//                             title: const Text('Don\'t know'),
//                             value: 0,
//                             groupValue: _result,
//                             onChanged: (int? value) {
//                               setState(() {
//                                 _result = value;
//                               });
//                             },
//                           ),
//                           RadioListTile<int>(
//                             title: const Text('A1'),
//                             value: 1,
//                             groupValue: _result,
//                             onChanged: (int? value) {
//                               setState(() {
//                                 _result = value;
//                               });
//                             },
//                           ),
//                           RadioListTile<int>(
//                             title: const Text('A2'),
//                             value: 2,
//                             groupValue: _result,
//                             onChanged: (int? value) {
//                               setState(() {
//                                 _result = value;
//                               });
//                             },
//                           ),
//                           RadioListTile<int>(
//                             title: const Text('B1'),
//                             value: 3,
//                             groupValue: _result,
//                             onChanged: (int? value) {
//                               setState(() {
//                                 _result = value;
//                               });
//                             },
//                           ),
//                           RadioListTile<int>(
//                             title: const Text('B2'),
//                             value: 4,
//                             groupValue: _result,
//                             onChanged: (int? value) {
//                               setState(() {
//                                 _result = value;
//                               });
//                             },
//                           ),
//                           RadioListTile<int>(
//                             title: const Text('C1'),
//                             value: 5,
//                             groupValue: _result,
//                             onChanged: (int? value) {
//                               setState(() {
//                                 _result = value;
//                               });
//                             },
//                           ),
//                           RadioListTile<int>(
//                             title: const Text('Select words by own hands'),
//                             value: 10,
//                             groupValue: _result,
//                             onChanged: (int? value) {
//                               setState(() {
//                                 _result = value;
//                               });
//                             },
//                           ),
//                         ],
//                       );
//               } else {
//                 return const Center(
//                   child: CircularProgressIndicator(
//                     color: Colors.blueGrey,
//                   ),
//                 );
//               }
//             },
//           )),
//     );
//   }
// }
