# Ammo Tweaks
Un patcher configurable conçu comme une alternative WACCF-friendly à ABT.
Rééchelonne les dégâts des projectiles entre un minimum et un maximum configurables tout en supprimant leur poids et en ajustant leur vitesse et leur gravité.
Peut éventuellement renommer les projectiles pour un meilleur tri et ajuster le nombre d'entre elles récupérables sur les corps.

## Paramètres
Fichier de configuration (Data/config.json), les paramètres sont configuables via synthesis
- Rééchelonner : Active la nouvelle échelle des dégâts. Mettre à false pour désactiver.
- Dégâts minimaux : Dégâts minimums autorisés.
- Dégâts maximaux : Dégâts maximums autorisés.
- Multiplicateur : Modifie la quantité de projectiles trouvés sur les corps. Les valeurs inférieures à 1 réduiront la quantité de projectiles trouvés, tandis que les valeurs supérieures à 1 l'augmenteront. Mettre à 1 pour désactiver.
- Renommer : Utilisé pour renommer les munitions en utilisant le schéma suivant : "Iron Arrow" -> "Arrow - Iron". Mettre à true pour activer.(Paramètres inutiles en français)
- Séparateur : Séparateur utilisé si le renommage est activé. Par exemple, " - " ou " : ".
- Modifier les projectiles : Active les modifications de la vitesse et de la gravité des projectiles. Mettez false pour désactiver.
- Vitesse des flèches : Nouvelle vitesse des flèches.
- Vitesse des carreaux: Nouvelle vitesse des carreaux.
- Gravité : Nouvelle valeur de gravité pour les flèches et les carreaux.

## Version à utiliser
0.31 et 0.19.3

# Crédits
Un très grand merci à Phlasriel qui a gentiment modifié le code pour qu'il fonctionne avec les particularités de la langue française.
