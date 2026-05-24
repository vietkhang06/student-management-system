package com.demo.student_management.security;

import com.demo.student_management.entity.TaiKhoan;
import com.demo.student_management.repository.TaiKhoanRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import javax.crypto.Mac;
import javax.crypto.spec.SecretKeySpec;
import java.nio.charset.StandardCharsets;
import java.time.Instant;
import java.util.Base64;
import java.util.Optional;

@Service
@RequiredArgsConstructor
public class AuthTokenService {

    private static final String HMAC_ALGORITHM = "HmacSHA256";

    private final TaiKhoanRepository taiKhoanRepository;

    @Value("${app.security.token-secret:student-management-dev-secret-change-me}")
    private String tokenSecret;

    @Value("${app.security.token-ttl-seconds:28800}")
    private long tokenTtlSeconds;

    public String createToken(TaiKhoan taiKhoan) {
        long expiresAt = Instant.now().plusSeconds(tokenTtlSeconds).getEpochSecond();
        String payload = taiKhoan.getIdTaiKhoan() + ":" + expiresAt;
        return encode(payload) + "." + sign(payload);
    }

    public Optional<TaiKhoan> resolveToken(String token) {
        if (token == null || token.isBlank()) {
            return Optional.empty();
        }

        String[] parts = token.split("\\.", 2);
        if (parts.length != 2) {
            return Optional.empty();
        }

        String payload;
        try {
            payload = new String(Base64.getUrlDecoder().decode(parts[0]), StandardCharsets.UTF_8);
        } catch (IllegalArgumentException ex) {
            return Optional.empty();
        }

        if (!sign(payload).equals(parts[1])) {
            return Optional.empty();
        }

        String[] payloadParts = payload.split(":", 2);
        if (payloadParts.length != 2) {
            return Optional.empty();
        }

        long expiresAt;
        try {
            expiresAt = Long.parseLong(payloadParts[1]);
        } catch (NumberFormatException ex) {
            return Optional.empty();
        }

        if (expiresAt < Instant.now().getEpochSecond()) {
            return Optional.empty();
        }

        return taiKhoanRepository.findById(payloadParts[0]);
    }

    private String encode(String payload) {
        return Base64.getUrlEncoder()
                .withoutPadding()
                .encodeToString(payload.getBytes(StandardCharsets.UTF_8));
    }

    private String sign(String payload) {
        try {
            Mac mac = Mac.getInstance(HMAC_ALGORITHM);
            mac.init(new SecretKeySpec(tokenSecret.getBytes(StandardCharsets.UTF_8), HMAC_ALGORITHM));
            return Base64.getUrlEncoder()
                    .withoutPadding()
                    .encodeToString(mac.doFinal(payload.getBytes(StandardCharsets.UTF_8)));
        } catch (Exception ex) {
            throw new IllegalStateException("Khong the tao chu ky token", ex);
        }
    }
}
