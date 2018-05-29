DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `giveRep`(in_id BIGINT, by_id BIGINT, reason VARCHAR(120), rep VARCHAR(4), nameAndDisc VARCHAR(85))
IF EXISTS(SELECT * FROM `rep-bot`.playreps WHERE playerId = in_id AND reppedById = by_id) THEN
	DELETE FROM `rep-bot`.`playreps` WHERE playerId = in_id AND reppedById = by_id;
    INSERT INTO `rep-bot`.`playreps` (playerId, reppedById, reasonGiven, repGiven, userAndDiscriminator) VALUES (in_id, by_id, reason, rep, nameAndDisc);
    
    ELSE
    INSERT INTO `rep-bot`.`playreps` (playerId, reppedById, reasonGiven, repGiven, userAndDiscriminator) VALUES (in_id, by_id, reason, rep, nameAndDisc);
    select *;
END IF$$
DELIMITER ;