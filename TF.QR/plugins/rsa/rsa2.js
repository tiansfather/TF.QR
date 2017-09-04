
// Return the PKCS#1 RSA decryption of "ctext".
// "ctext" is an even-length hex string and the output is a plain string.
function RSADecrypt(ctext, N, E) {
    var n = parseBigInt(N, 16);
    var e = parseBigInt(E, 16);
    var c = parseBigInt(ctext, 16);
    var m = c.modPow(e, n);
    if (m == null) return null;
    return bigintToText(m);
}
RSAKey.prototype.decrypt = RSADecrypt;
//RSAKey.prototype.b64_decrypt = RSAB64Decrypt;


function bigintToText(d) {
    var b = d.toByteArray();
    var i = -1;
    var ret = "";
    while (++i < b.length) {
        var c = b[i] & 255;
        if (c < 128) { // utf-8 decode
            ret += String.fromCharCode(c);
        }
        else if ((c > 191) && (c < 224)) {
            ret += String.fromCharCode(((c & 31) << 6) | (b[i + 1] & 63));
            ++i;
        }
        else {
            ret += String.fromCharCode(((c & 15) << 12) | ((b[i + 1] & 63) << 6) | (b[i + 2] & 63));
            i += 2;
        }
    }
    return ret;
}
function rsaDecrypt2(text, N, E) {
    var n = parseBigInt(N, 16);
    var e = parseBigInt(E, 16);
    var c = parseBigInt(text, 16);
    var m = c.modPow(e, n);
    if (m == null) return null;
    return bigintToText(m);
}


