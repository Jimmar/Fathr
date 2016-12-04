HOW TO MAKE XML FILE.

FOR WORDS:

- Make one for every word that Dad doesn't understand and one for every word used as an image tag word.
- Copy-paste another one. Change the filename to something readable.
- In the <BaseWord> tag, replace the word with the word that this corresponds to. I think capitalization matters...I don't remember.
- <LinkedWords> lists the words the will show up in the word bank for this word, i.e., related concepts.
--- Each word has a weight, from 0.0 to 1.0, which notes how related it is to the concept.
--- E.g. For the BaseWord "Anime", <Link word="Japanese" weight="1.0"/>.
--- Another way to frame it is the weight is how much Dad will understand the BaseWord if he understands this LinkedWord.
- <Descriptors> lists adjectives describing how people would feel about the image. Things like "cute," "funny," or "ironic."
--- They also have weights that are similar; if dad associates the descriptor with the BaseWord, he'll be <weight> correct.
--- E.g. Sonic porn: <Link word="sexy" weight="1.0"/> <Link word="cute" weight=0.3/>
- <UnderstandingBase> is a number from 0.0 to 1.0 noting how much Dad understands the base word.
--- If UnderstandingBase is 1.0 and the word is not an image word, there's no need to make a file for it.
--- E.g. We don't have any pictures featuring corn, so even if the BaseWord "vegetable" has "corn" as a LinkedWord, "corn" does not need its own file since Dad understands corn.
- <UnderstandingByDadType> is unused.

FOR IMAGES:

- Similar to words, except there's an <Image> instead of a <BaseWord>. This Image has a fileName that should match the file name of the image.
- The image and XML file should be in the same folder.
- <LinkedWord> weights here correspond to how much Dad needs to understand that word in order to get the image.
--- E.g. for Sonic porn, Dad doesn't really need to know much about Sonic, just that he's a hedgehog from a video game, so maybe its weight is 0.5. But, he definitely needs to know what porn is, so porn's weight is 1.0.
-<Descriptors> function similarly to words.

